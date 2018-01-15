using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Android.Graphics;
using Models.DTO;
using OdborkyApp.Tools;
using Plugin.Connectivity;
using Path = System.IO.Path;

namespace OdborkyApp.Service
{
    public class SyncService
    {
        public event EventHandler PullSuccess;
        public event EventHandler PushSuccess;
        public event EventHandler SyncSuccess;
        public event EventHandler<Exception> SyncFail;

        private readonly DateTime? _lastSyncTime;
        private readonly string _challengesDir;
        private readonly ChallengeAppHttpClient _client;

        public SyncService(DateTime? lastSyncTime = null)
        {
            _lastSyncTime = lastSyncTime;
            _client = new ChallengeAppHttpClient(_lastSyncTime);

            _challengesDir = AppState.State.Instance.ChallengesPath;
            if (!Directory.Exists(_challengesDir))
            {
                Directory.CreateDirectory(_challengesDir);
            }
        }

        public void Synchronize()
        {
            try
            {
                Pull();
                Push();
                SyncSuccess?.Invoke(this, null);
            }
            catch (Exception e)
            {
                if (AppState.State.Instance.Challenges == null)
                {
                    var oldChallenges = LoadChallengesFromFiles(Directory.GetFiles(_challengesDir).Skip(1).ToArray());
                    AppState.State.Instance.Challenges = new List<Challenge>(oldChallenges);
                }
                SyncFail?.Invoke(this, e);
            }
        }

        public void Pull()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var fetchedUsers = _client.FetchCollection<User>("users", "User");
                var fetchedChallenges = _client.FetchCollection<Challenge>("challenges", "Challenge");
                var fetchedChallengeProgess =
                    _client.FetchCollection<ChallengeProgress>("progress/challenges", "ChallengeProgress");
                var fetchedTaskProgess = _client.FetchCollection<TaskProgress>("progress/tasks", "TaskProgress");

                SyncChallenges(fetchedChallenges);
                SyncCollection(fetchedUsers);
                SyncCollection(fetchedChallengeProgess);
                SyncCollection(fetchedTaskProgess);

                PullSuccess?.Invoke(this, null);
            }
            else
            {
                throw new NotConnectedException();
            }
        }

        public void Push()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
                {
                    var challengeProgresses = db.Table<ChallengeProgress>().ToList()
                        .Where((c) => c.LastModified > _lastSyncTime);

                    var taskProgresses = db.Table<TaskProgress>().ToList()
                        .Where((t) => t.LastModified > _lastSyncTime);

                    _client.PutCollectionByElements("progress/challenges", challengeProgresses, (c) => c.Id.ToString());
                    _client.PutCollectionByElements("progress/tasks", taskProgresses, (t) => t.Id.ToString());
                }

                PushSuccess?.Invoke(this, null);
            }
            else
            {
                throw new NotConnectedException();
            }
        }

        private IEnumerable<Challenge> LoadChallengesFromFiles(string[] challengeFileNames)
        {
            var loadedChallenges = new List<Challenge>();
            XmlSerializer serializer = new XmlSerializer(typeof(Challenge));

            foreach (var fileName in challengeFileNames)
            {
                var filePath = Path.Combine(_challengesDir, fileName);

                using (var fs = new FileStream(filePath, FileMode.Open))
                using (var reader = XmlReader.Create(fs))
                {
                    loadedChallenges.Add((Challenge)serializer.Deserialize(reader));
                }
            }

            return loadedChallenges;
        }

        private IEnumerable<Challenge> ScrapeImages(IEnumerable<Challenge> fetchedChallenges)
        {
            var imagesDirPath = Path.Combine(_challengesDir, "images");

            if (!Directory.Exists(imagesDirPath))
            {
                Directory.CreateDirectory(imagesDirPath);
            }

            var withImages = fetchedChallenges as Challenge[] ?? fetchedChallenges.ToArray();
            foreach (var challenge in withImages)
            {
                var imagePath = Path.Combine(
                   imagesDirPath,
                   challenge.Names[0].Name + ".png");

                challenge.ImageUri.LocalPath = imagePath;

                var imageRequest = new HttpWebRequest(new System.Uri(challenge.ImageUri.Uri))
                {
                    Method = "GET"
                };

                using (var imageResponse = imageRequest.GetResponse())
                using (var imageResponseStream = imageResponse.GetResponseStream())
                {
                    var image = BitmapFactory.DecodeStream(imageResponseStream);

                    using (var imageFileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        image.Compress(Bitmap.CompressFormat.Png, 100, imageFileStream);
                    }
                }
            }

            return withImages;
        }

        private void StoreFetchedChallenges(IEnumerable<Challenge> fetchedChallenges)
        {
            var withImages = ScrapeImages(fetchedChallenges);
            var serializer = new XmlSerializer(typeof(Challenge));

            foreach (var challenge in withImages)
            {
                var filePath = Path.Combine(
                    _challengesDir,
                    challenge.Names[0].Name + ".xml");

                using (var fs = new FileStream(filePath, FileMode.Create))
                using (var writer = XmlWriter.Create(fs))
                {
                    serializer.Serialize(writer, challenge);
                }
            }
        }

        private void SyncChallenges(IEnumerable<Challenge> fetchedChallenges)
        {
            var oldChallengesFilePaths = Directory.GetFiles(_challengesDir).Skip(1).ToArray();
            var newChallenges = fetchedChallenges as Challenge[] ?? fetchedChallenges.ToArray();
            StoreFetchedChallenges(newChallenges);

            List<string> newFilePaths = new List<string>();
            foreach (var challenge in newChallenges)
            {
                var filePath = Path.Combine(
                    _challengesDir,
                    challenge.Names[0].Name + ".xml");

                newFilePaths.Add(filePath);
            }

            var match = oldChallengesFilePaths.Except(newFilePaths).ToArray();
            var oldChallenges = LoadChallengesFromFiles(match);

            var allChallenges = new List<Challenge>(oldChallenges);
            allChallenges.AddRange(newChallenges);

            if (AppState.State.Instance.Challenges == null)
            {
                AppState.State.Instance.Challenges = new List<Challenge>(allChallenges);
            }
        }

        private void SyncCollection<T>(IEnumerable<T> fetchedCollection)
        {
            using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
            {
                foreach (var entry in fetchedCollection)
                {
                    db.InsertOrReplace(entry, typeof(T));
                }
            }
        }
    }

    internal class NotConnectedException : Exception
    {
        public NotConnectedException() : base("Připojení k internetu není k dispozici") { }
    }
}