using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Models.DTO;
using ChallengeProgress = OdborkyApp.Model.ChallengeProgress;
using TaskProgress = OdborkyApp.Model.TaskProgress;

namespace OdborkyApp.Components
{
    internal class TaskComponent
    {
        public View Container { get; private set; }
        private ChallengeTask Task { get; set; }
        private TaskProgress TaskProgress { get; set; }
        private ChallengeProgress ChallengeProgress { get; set; }

        public event EventHandler<TaskProgress> TaskStarted;
        public event EventHandler<TaskProgress> TaskConfirmationRequested;
        public event EventHandler<View> Invalidated;

        private Context _context;

        public TaskComponent(Context ctx, ChallengeTask task, ChallengeProgress challengeProgress, TaskProgress taskProgress)
        {
            _context = ctx;
            Task = task;
            ChallengeProgress = challengeProgress;
            TaskProgress = taskProgress;
            Container = ConstructView();
        }

        public void Invalidate()
        {
            var oldContainer = Container;
            Container = ConstructView();
            Invalidated.Invoke(this, oldContainer);
        }

        private View ConstructView()
        {
            var outerLayout = new LinearLayout(_context)
            {
                Orientation = Orientation.Vertical
            };
            outerLayout.SetPadding(20, 0, 20, 10);

            var taskNameView = new TextView(_context);
            taskNameView.SetTextSize(Android.Util.ComplexUnitType.Pt, 9);

            if (TaskProgress != null)
            {
                if (TaskProgress.Status > Models.ProgressStatus.InProgress)
                {
                    taskNameView.Text = "✔ ";
                    outerLayout.SetBackgroundColor(Color.LightGreen);
                }
                else
                {
                    taskNameView.Text = "● ";
                    outerLayout.SetBackgroundColor(Color.LightYellow);
                }
            }
            else
                taskNameView.Text = "○ ";

            taskNameView.Text += Task.Name;

            var taskRegionLayout = new LinearLayout(_context)
            {
                Orientation = Orientation.Vertical,
                Visibility = ViewStates.Gone
            };

            var taskDescTextView = new TextView(_context)
            {
                Text = Task.Description
            };
            taskDescTextView.SetTextSize(Android.Util.ComplexUnitType.Pt, 8);

            taskRegionLayout.AddView(taskDescTextView);

            if (ChallengeProgress?.Status == Models.ProgressStatus.InProgress)
            {
                if (TaskProgress == null)
                {
                    var taskBtn = new Button(_context);

                    taskBtn.SetBackgroundColor(Color.Orange);
                    taskBtn.Text = "Začít plnit úkol!";

                    taskBtn.Click += (o, e) =>
                    {
                        TaskProgress = new TaskProgress
                        {
                            Id = Guid.NewGuid(),
                            TaskId = Task.Id,
                            LastModified = DateTime.UtcNow,
                            ChallengeProgressId = ChallengeProgress.Id,
                            Status = Models.ProgressStatus.InProgress
                        };

                        TaskStarted.Invoke(this, TaskProgress);
                        Invalidate();
                    };

                    taskRegionLayout.AddView(taskBtn);
                }
                else if (TaskProgress.Status == Models.ProgressStatus.InProgress)
                {
                    var taskBtn = new Button(_context);

                    taskBtn.SetBackgroundColor(Color.LightGreen);
                    taskBtn.Text = "Zažádat o potvrzení úkolu!";

                    taskBtn.Click += (o, e) =>
                    {
                        TaskConfirmationRequested.Invoke(this, TaskProgress);
                    };

                    taskRegionLayout.AddView(taskBtn);
                }
            }

            taskNameView.Click += (o, e) =>
            {
                if (taskRegionLayout.Visibility == ViewStates.Gone)
                    taskRegionLayout.Visibility = ViewStates.Visible;
                else
                    taskRegionLayout.Visibility = ViewStates.Gone;
            };

            outerLayout.AddView(taskNameView);
            outerLayout.AddView(taskRegionLayout);

            return outerLayout;
        }
    }
}