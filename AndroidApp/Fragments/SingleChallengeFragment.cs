using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Models;
using Models.DTO;
using OdborkyApp.Activities;
using OdborkyApp.AppState;
using OdborkyApp.Base;
using OdborkyApp.Components;
using OdborkyApp.Model;

namespace OdborkyApp.Fragments
{
    public class SingleChallengeFragment : TitledFragment
    {
        private readonly Challenge _selectedChallenge;
        private Model.ChallengeProgress _challengeProgress;
        private readonly IEnumerable<Model.TaskProgress> _tasksProgresses;
        private readonly List<TaskComponent> _taskComponents = new List<TaskComponent>();

        private readonly int _basicTasksCompleted;
        private readonly int _extraTasksCompleted;
        private (int Basic, int Extra) _currentRequirements;

        private Bitmap _challengeImage;
        private bool _invalidated = false;
        private int _challlengeIndex;


        public SingleChallengeFragment(int challengeIndex)
        {
            _challlengeIndex = challengeIndex;
            _selectedChallenge = AppState.State.Instance.Challenges.ElementAt(challengeIndex);
            _currentRequirements = GetRequirements(AppState.State.Instance.UserDetails.CategoryId);

            using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
            {
                _challengeProgress = db.Table<Model.ChallengeProgress>().ToList()
                    .FirstOrDefault(c => c.ChallengeId == _selectedChallenge.Id && c.UserId == State.Instance.UserDetails.UserId);

                if (_challengeProgress != null)
                {
                    _tasksProgresses = db.Table<Model.TaskProgress>().ToList()
                        .Where((t) => t.ChallengeProgressId == _challengeProgress.Id).ToList();

                    _basicTasksCompleted = _tasksProgresses
                        .Join(_selectedChallenge.BasicTasks, (t) => t.TaskId, (bt) => bt.Id, (t, bt) => 
                        new { TaskId = t.TaskId, Status = t.Status }).Count(st => st.Status >= ProgressStatus.Completed);

                    _extraTasksCompleted = _tasksProgresses
                        .Join(_selectedChallenge.ExtraTasks, (t) => t.TaskId, (bt) => bt.Id, (t, bt) => 
                        new { TaskId = t.TaskId, Status = t.Status }).Count(st => st.Status >= ProgressStatus.Completed);
                }
            }

            Title = _selectedChallenge.Names[0].Name;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _challengeImage = BitmapFactory.DecodeFile(_selectedChallenge.ImageUri.LocalPath);
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_invalidated)
            {
                _invalidated = false;
                FragmentManager
                    .BeginTransaction()
                    .Remove(this)
                    .Replace(Resource.Id.frameLayout1, new SingleChallengeFragment(_challlengeIndex))
                    .AddToBackStack(null)
                    .Commit();
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var fragmentView = inflater.Inflate(Resource.Layout.ChallengeLayout, container, false);
            var bitmapView = fragmentView.FindViewById<ImageView>(Resource.Id.challengeIconView);

            var nameTextView = fragmentView.FindViewById<TextView>(Resource.Id.challengeNameTextView);
            var descTextView = fragmentView.FindViewById<TextView>(Resource.Id.challengeDesctextView);
            var requirementTextView = fragmentView.FindViewById<TextView>(Resource.Id.requirementsTextView);

            var basicTaskRegionView = fragmentView.FindViewById<LinearLayout>(Resource.Id.basicTasksRegion);
            var extraTaskRegionView = fragmentView.FindViewById<LinearLayout>(Resource.Id.extraTasksRegion);

            var basicTaskRegionLabel = fragmentView.FindViewById<TextView>(Resource.Id.basicTasksIntroText);
            var extraTaskRegionLabel = fragmentView.FindViewById<TextView>(Resource.Id.extraTasksIntroText);

            var startChallenge = fragmentView.FindViewById<Button>(Resource.Id.startChallenge);

            using (var db = new SQLite.SQLiteConnection(State.Instance.DbPath))
            {
                _challengeProgress = db.Table<Model.ChallengeProgress>()
                    .ToList()
                    .FirstOrDefault(c => c.ChallengeId == _selectedChallenge.Id
                        && c.UserId == AppState.State.Instance.UserDetails.UserId);


                if (_challengeProgress?.Status == ProgressStatus.InProgress
                    && _basicTasksCompleted >= _currentRequirements.Basic
                    && _extraTasksCompleted >= _currentRequirements.Extra)
                {
                    startChallenge.Text = "Zažádat o potvrzení!";
                    startChallenge.SetBackgroundColor(Color.LightGreen);
                    requirementTextView.Visibility = ViewStates.Gone;

                    startChallenge.Click += (o, e) =>
                    {
                        var request = new ConfirmationRequest
                        {
                            ChallengeProgressId = _challengeProgress.Id,
                            TargetId = _selectedChallenge.Id,
                            Type = ConfirmationType.Challenge,
                            UserId = AppState.State.Instance.UserDetails.UserId
                        };
                        var confirmIntent = new Intent(container.Context, typeof(QrActivity));
                        var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                        confirmIntent.PutExtra("requestJson", requestJson);
                        confirmIntent.PutExtra("scanEnum", (int)ScannerRequestEnum.DisplayRequest);

                        StartActivity(confirmIntent);
                        _invalidated = true;
                    };
                }
                else if (_challengeProgress == null)
                {
                    startChallenge.Text = "Začít plnit zkoušku!";
                    startChallenge.SetBackgroundColor(Color.Orange);
                    requirementTextView.Visibility = ViewStates.Gone;

                    startChallenge.Click += (o, e) =>
                    {
                        using (var dbOnClick = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
                        {
                            var progress = new Model.ChallengeProgress
                            {
                                Id = Guid.NewGuid(),
                                ChallengeId = _selectedChallenge.Id,
                                Status = ProgressStatus.InProgress,
                                LastModified = DateTime.UtcNow,
                                UserId = AppState.State.Instance.UserDetails.UserId,
                            };
                            dbOnClick.Insert(progress);

                            FragmentManager.BeginTransaction().Detach(this).Attach(this).Commit();
                        }
                    };
                }
                else
                {
                    requirementTextView.Visibility = ViewStates.Visible;
                    startChallenge.Visibility = ViewStates.Gone;
                }
            }

            bitmapView.SetImageBitmap(_challengeImage);

            requirementTextView.Text = GetProgressString();

            nameTextView.Text = _selectedChallenge.Names[0].Name;

            basicTaskRegionLabel.Click += (o, e) =>
            {
                if (basicTaskRegionView.Visibility == ViewStates.Visible)
                {
                    basicTaskRegionView.Visibility = ViewStates.Gone;
                    basicTaskRegionLabel.Text = "▼" + new string(basicTaskRegionLabel.Text.Skip(1).ToArray());
                }
                else
                {
                    basicTaskRegionView.Visibility = ViewStates.Visible;
                    basicTaskRegionLabel.Text = "▲" + new string(basicTaskRegionLabel.Text.Skip(1).ToArray());
                }
            };

            extraTaskRegionLabel.Click += (o, e) =>
            {
                if (extraTaskRegionView.Visibility == ViewStates.Visible)
                {
                    extraTaskRegionView.Visibility = ViewStates.Gone;
                    extraTaskRegionLabel.Text = "▼" + new string(extraTaskRegionLabel.Text.Skip(1).ToArray());
                }
                else
                {
                    extraTaskRegionView.Visibility = ViewStates.Visible;
                    extraTaskRegionLabel.Text = "▲" + new string(extraTaskRegionLabel.Text.Skip(1).ToArray());
                }
            };

            foreach (var bTask in _selectedChallenge.BasicTasks.Select((t, i) => new { Task = t, Index = i}))
            {
                var taskComponent = CreateComponent(container.Context, bTask.Task, bTask.Index, basicTaskRegionView);

                _taskComponents.Add(taskComponent);
                basicTaskRegionView.AddView(taskComponent.Container);
            }

            foreach (var eTask in _selectedChallenge.ExtraTasks.Select((t, i) => new { Task = t, Index = i }))
            {
                var taskComponent = CreateComponent(container.Context, eTask.Task, eTask.Index, extraTaskRegionView);

                _taskComponents.Add(taskComponent);
                extraTaskRegionView.AddView(taskComponent.Container);
            }

            descTextView.Text = _selectedChallenge.Description;
            
            return fragmentView;
        }

        private TaskComponent CreateComponent(Context ctx, ChallengeTask task, int componentIndex, LinearLayout container)
        {
            var taskComponent = new TaskComponent(ctx, task, _challengeProgress,
                _tasksProgresses?.FirstOrDefault((t) => t.TaskId == task.Id));

            taskComponent.Invalidated += (o, formerView) =>
            {
                container.RemoveView(formerView);
                container.AddView(taskComponent.Container, componentIndex);
            };

            taskComponent.TaskStarted += (o, taskProgress) =>
            {
                using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
                {
                    db.Insert(taskProgress);
                };
            };

            taskComponent.TaskConfirmationRequested += (o, taskProgress) =>
            {
                var request = new ConfirmationRequest
                {
                    TaskProgressId = taskProgress.Id,
                    ChallengeProgressId = _challengeProgress.Id,
                    TargetId = task.Id,
                    Type = ConfirmationType.Task,
                    UserId = AppState.State.Instance.UserDetails.UserId
                };

                var confirmIntent = new Intent(ctx, typeof(QrActivity));
                var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                confirmIntent.PutExtra("requestJson", requestJson);

                StartActivity(confirmIntent);
                _invalidated = true;
            };

            return taskComponent;
        }

        private (int Basic, int Extra) GetRequirements(MembershipCategoryId category)
        {
  
            switch (category)
            {
                case MembershipCategoryId.Cub:
                    return (_selectedChallenge.BasicRequirements.Cubs, _selectedChallenge.ExtraRequirements.Cubs);

                case MembershipCategoryId.Scout:
                    return (_selectedChallenge.BasicRequirements.Scouts, _selectedChallenge.ExtraRequirements.Scouts);

                default:
                    return(_selectedChallenge.BasicRequirements.Guides, _selectedChallenge.ExtraRequirements.Guides);
            }
        }

        private string GetProgressString()
        {
            int count = _currentRequirements.Basic + _currentRequirements.Extra;
            int delim = _currentRequirements.Basic;
            string s = "";

            for (int i = 0; i < count; i++)
            {
                if (i < _basicTasksCompleted && i < delim || i - delim < _extraTasksCompleted && i > delim - 1)
                    s += " ● ";
                else
                    s += " ○ ";

                if (i == delim - 1)
                    s += (count > 10 ? '\n' : '|');
            }

            return s;
        }
    }
}        