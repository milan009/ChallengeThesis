using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Widget;
using Models;
using Models.DTO;
using OdborkyApp.Base;
using OdborkyApp.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using ZXing;
using ZXing.QrCode;
using ChallengeProgress = OdborkyApp.Model.ChallengeProgress;
using TaskProgress = OdborkyApp.Model.TaskProgress;

namespace OdborkyApp.Activities
{
    [Activity(Label = "QRActivity")]
    public class QrActivity : LoadedChallengesBasedActivity
    {
        private EventHandler<ConfirmationRequest> _onScanRequest;
        private EventHandler<Confirmation> _onScanConfirmation;

        protected override void OnChallengesLoaded()
        {
            switch ((ScannerRequestEnum) Intent.GetIntExtra("scanEnum", 0))
            {
                case ScannerRequestEnum.DisplayRequest:
                {
                    var requestJson = Intent.GetStringExtra("requestJson");
                    DisplayRequest(requestJson);
                    break;
                }

                case ScannerRequestEnum.ScanRequest:
                {
                    _onScanRequest += ProcessRequest;
                    ScanRequest();
                    break;
                }

                case ScannerRequestEnum.ScanConfirmation:
                {
                    _onScanConfirmation += ProcessConfirmation;
                    ScanConfirmation();
                    break;
                }
            }
        }

        // UI initialization

        private void DisplayRequest(string requestJson)
        {
            var writer = new ZXing.Mobile.BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = new QrCodeEncodingOptions();
            writer.Options.Width = 512;
            writer.Options.Height = 512;

            var code = writer.Write(requestJson);
            var requestObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfirmationRequest>(requestJson);

            SetContentView(Resource.Layout.QrLayout);

            var imageView = FindViewById<ImageView>(Resource.Id.qrImageView);
            imageView.SetImageBitmap(code);

            var btn = FindViewById<Button>(Resource.Id.qrButton);

            btn.Click += (o, e) => ScanConfirmation();
            _onScanConfirmation += ProcessConfirmation;

            var allTasks = new List<ChallengeTask>();
            foreach (var c in AppState.State.Instance.Challenges)
            {
                allTasks.AddRange(c.BasicTasks);
                allTasks.AddRange(c.ExtraTasks);
            }

            var targetName = (requestObject.Type == ConfirmationType.Challenge
                ? AppState.State.Instance.Challenges
                    .ToList()[requestObject.TargetId].Names[0].Name
                : allTasks.FirstOrDefault(t => t.Id == requestObject.TargetId)?.Name);

            var text = FindViewById<TextView>(Resource.Id.qrScanRequestText);
            text.Text = $"Žádost o potvrzení " 
                + (requestObject.Type == ConfirmationType.Challenge ? "zkoušky " : "úkolu ") + targetName;
        }

        private void DisplayConfirmation(string confString)
        {
            var writer = new ZXing.Mobile.BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = new QrCodeEncodingOptions();
            writer.Options.Width = 512;
            writer.Options.Height = 512;

            var code = writer.Write(confString);
            SetContentView(Resource.Layout.QrLayout);

            var confObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Confirmation>(confString);

            var imageView = FindViewById<ImageView>(Resource.Id.qrImageView);
            imageView.SetImageBitmap(code);

            var btn = FindViewById<Button>(Resource.Id.qrButton);
            btn.Text = "Zpìt do menu";

            var allTasks = new List<ChallengeTask>();
            foreach (var c in AppState.State.Instance.Challenges)
            {
                allTasks.AddRange(c.BasicTasks);
                allTasks.AddRange(c.ExtraTasks);
            }

            var targetName = (confObject.Request.Type == ConfirmationType.Challenge
                ? AppState.State.Instance.Challenges
                    .ToList()[confObject.Request.TargetId].Names[0].Name
                : allTasks.FirstOrDefault(t => t.Id == confObject.Request.TargetId)?.Name);

            Model.User targetUser;
            using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
            {
                targetUser = db.Find<Model.User>(confObject.Request.UserId);
            }

                var text = FindViewById<TextView>(Resource.Id.qrScanRequestText);
            text.Text = "Potvrzení " + (confObject.Request.Type == ConfirmationType.Challenge ? "zkoušky " : "úkolu ") + targetName
                + " pro uživatele " + targetUser.Name;

            btn.Click += (o, e) =>
            {
                SetResult(Android.App.Result.Ok);
                Finish();
            };
        }


        // Scanning

        private async void ScanRequest()
        {
            try
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan(this, ZXing.Mobile.MobileBarcodeScanningOptions.Default);

                if (result != null)
                    _onScanRequest.Invoke(this, Newtonsoft.Json.JsonConvert.DeserializeObject<ConfirmationRequest>(result.Text));                  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async void ScanConfirmation()
        {
            try
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan(this, ZXing.Mobile.MobileBarcodeScanningOptions.Default);

                if (result != null)
                    _onScanConfirmation.Invoke(this, Newtonsoft.Json.JsonConvert.DeserializeObject<Confirmation>(result.Text));
            }
            catch (Exception e)
            {

                new AlertDialog.Builder(this)
                    .SetMessage("Pøi skenování došlo k chybì: " + e.Message)
                    .SetNeutralButton("Ok", (o, ev) =>
                    {
                        SetResult(Android.App.Result.Canceled);
                        Finish();
                    })
                    .Create()
                    .Show();
            }
        }


        // Processing

        private void ProcessRequest(object sender, ConfirmationRequest request)
        {
            try
            {
                using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
                {
                    if (request.UserId == AppState.State.Instance.UserDetails.UserId)
                    {
                        throw new ConfirmationException("Nemùžete potvrzovat vlastní požadavky");
                    }

                    var r = db.Table<Model.User>();
                    var l = db.Get<Model.User>(request.UserId);

                    var targetUser = db.Find<Model.User>(request.UserId);
                    if (targetUser == null)
                    {
                        throw new ConfirmationException("Vaše zaøízení tohoto uživatele nerozpoznalo. Jeho požadavek nelze potvrdit.");
                    }

                    string dialogText;
                    if (request.Type == ConfirmationType.Challenge)
                    {
                        var challenge = AppState.State.Instance.Challenges.FirstOrDefault((c) => c.Id == request.TargetId);
                        if (challenge == null)
                        {
                            throw new ConfirmationException("Neznámá zkouška");
                        }

                        dialogText = $"Uživatel { targetUser.Name} žádá o potvrzení zkoušky { challenge.Names[0].Name}. Potvrdit?";
                    }
                    else
                    {
                        ChallengeTask task = null;

                        foreach(var challenge in AppState.State.Instance.Challenges)
                        {
                            if ((task = challenge.BasicTasks.FirstOrDefault((t) => t.Id == request.TargetId)) != null)
                                break;

                            if ((task = challenge.ExtraTasks.FirstOrDefault((t) => t.Id == request.TargetId)) != null)
                                break;
                        }

                        if(task == null)
                        {
                            throw new ConfirmationException("Neznámý úkol");
                        }

                        dialogText = $"Uživatel { targetUser.Name} žádá o potvrzení úkolu { task.Name}. Potvrdit?";
                    }

                    new AlertDialog.Builder(this)
                        .SetTitle("Požadavek od uživatele")
                        .SetMessage(dialogText)
                        .SetPositiveButton("Ano", (o, e) => GenerateConfirmation(request))
                        .SetNegativeButton("Ne", CancelAction)
                        .Create()
                        .Show();
                    
                }
            }
            catch(Exception e)
            {
                new AlertDialog.Builder(this)
                    .SetMessage("Pøi ovìøování došlo k chybì: " + e.Message)
                    .SetNeutralButton("Ok", (o, ev) =>
                    {
                        SetResult(Android.App.Result.Canceled);
                        Finish();
                    })
                    .Create()
                    .Show();
            }
        }

        private void ProcessConfirmation(object sender, Confirmation response)
        {
            VerifyConfirmation(response);
        }

        private void GenerateConfirmation(ConfirmationRequest request)
        {
            var req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var signedMessage = Sign(req);
            var conf = new Confirmation { SignerId = AppState.State.Instance.UserDetails.UserId, Signature = signedMessage, Request = request };

            SaveConfirmedProgress(request);

            var str = Newtonsoft.Json.JsonConvert.SerializeObject(conf);
            DisplayConfirmation(str);
        }

        private void SaveConfirmedProgress(ConfirmationRequest request)
        {

            if (request.Type == ConfirmationType.Challenge)
            {
                var progress = new ChallengeProgress
                {
                    ChallengeId = request.TargetId,
                    Id = request.ChallengeProgressId,
                    LastModified = DateTime.UtcNow,
                    Status = ProgressStatus.Confirmed,
                    UserId = request.UserId
                };

                using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
                {
                    db.InsertOrReplace(progress);
                }
            }
            else
            {
                using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
                {
                    var challenge = AppState.State.Instance
                        .Challenges.FirstOrDefault((c) => c.BasicTasks.Any(t => t.Id == request.TargetId) || c.ExtraTasks.Any(t => t.Id == request.TargetId));
                    var challengeProgress = db
                        .Table<ChallengeProgress>()
                        .ToList()
                        .FirstOrDefault(c => c.ChallengeId == challenge.Id);

                    // If a user started a challenge and has not synced yet, confirming a task from that challenge
                    // could crash the app - thus, a new challenge progress object is created on the device of the
                    // confirming participant. On nearest synchronization, thi progress is passed to the db.

                    if (challengeProgress == null)
                    {
                        challengeProgress = new ChallengeProgress
                        {
                            ChallengeId = challenge.Id,
                            Id = request.ChallengeProgressId,
                            LastModified = DateTime.UtcNow,
                            Status = ProgressStatus.InProgress,
                            UserId = request.UserId,
                        };

                        db.InsertOrReplace(challengeProgress, typeof(ChallengeProgress));
                    }

                    var progress = new TaskProgress
                    {
                        TaskId = request.TargetId,
                        Id = request.TaskProgressId,
                        LastModified = DateTime.UtcNow,
                        Status = ProgressStatus.Confirmed,
                        ChallengeProgressId = request.ChallengeProgressId,
                    };

                    db.InsertOrReplace(progress, typeof(TaskProgress));
                }
            }
        }


        // Dialog response

        private void VerifyConfirmation(Confirmation confirmation)
        {
            var signerId = confirmation.SignerId;

            try
            {
                var res = Verify(Newtonsoft.Json.JsonConvert.SerializeObject(confirmation.Request),
                    confirmation.Signature, signerId);
                if (res)
                {
                    string responseText;
                    using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
                    {
                        if (confirmation.Request.Type == ConfirmationType.Task)
                        {
                            var tp = new TaskProgress
                            {
                                TaskId = confirmation.Request.TargetId,
                                ChallengeProgressId = confirmation.Request.ChallengeProgressId,
                                Id = confirmation.Request.TaskProgressId,
                                Status = ProgressStatus.Completed,
                                LastModified = DateTime.UtcNow,
                            };

                            db.InsertOrReplace(tp);

                            responseText = $"Splnìní úkolu úspìšnì potvrzeno! Gratulujeme";
                        }
                        else
                        {
                            var cp = new ChallengeProgress
                            {
                                ChallengeId = confirmation.Request.TargetId,
                                Id = confirmation.Request.ChallengeProgressId,
                                Status = ProgressStatus.Completed,
                                LastModified = DateTime.UtcNow,
                                UserId = confirmation.Request.UserId
                            };

                            db.InsertOrReplace(cp);

  
                            responseText = $"Splnìní zkoušky úspìšnì potvrzeno. Gratulujeme!";
                        }
                    }

                    new AlertDialog.Builder(this)
                        .SetMessage(responseText)
                        .SetNeutralButton("Ok", (o, e) =>
                        {
                            SetResult(Android.App.Result.Ok);
                            Finish();
                        })
                        .SetCancelable(false)
                        .Create()
                        .Show();
                }
                else
                {
                    throw new ConfirmationException("Elektronický podpis nesouhlasí.");
                }
            }
            catch (Exception e)
            {
                new AlertDialog.Builder(this)
                    .SetMessage($"Potvrzení nebylo úspìšné: {e.Message}")
                    .SetNeutralButton("Ok", (o, ev) =>
                    {
                        SetResult(Android.App.Result.Canceled);
                        Finish();
                    })
                    .SetCancelable(false)
                    .Create()
                    .Show();
            }
        }

        private void CancelAction(object sender, DialogClickEventArgs e)
        {
            SetResult(Android.App.Result.Canceled);
            Finish();
        }


        // Crypto

        private string Sign(string data)
        {
            var kc = new KeyChain.Net.XamarinAndroid.KeyChainHelper(() => this);
            var keyString = kc.GetKey("privateKey");

            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(keyString));
            var signer = SignerUtilities.GetSigner("SHA256withRSA");
            signer.Init(true, privateKey);

            var bytes = Encoding.UTF8.GetBytes(data);
            signer.BlockUpdate(bytes, 0, bytes.Length);

            byte[] signature = signer.GenerateSignature();

            var signedString = Convert.ToBase64String(signature);

            return signedString;
        }

        private bool Verify(string data, string expectedSignature, int signerId)
        {
            RsaKeyParameters signerKey;

            using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
            {
                var signerKeyEncoded = db.Find<Model.User>(signerId).PublicKeyEncoded;
                signerKey = (RsaKeyParameters) PublicKeyFactory.CreateKey(signerKeyEncoded);
            }

            ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");

            signer.Init(false, signerKey);

            var expectedSig = Convert.FromBase64String(expectedSignature);

            var msgBytes = Encoding.UTF8.GetBytes(data);

            signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
            return signer.VerifySignature(expectedSig);
        }

        internal class ConfirmationException : Exception
        {
            public ConfirmationException(string message) : base(message) { }
        }
    }
}