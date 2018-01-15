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
using OdborkyApp.Base;
using OdborkyApp.Components;
using ChallengeProgress = OdborkyApp.Model.ChallengeProgress;

namespace OdborkyApp.Fragments
{
    public class HomeFragment : TitledFragment
    {
        public event EventHandler<int> ChallengeSelected;
        public event EventHandler OverviewShortcutClicked;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = "Moje zkoušky";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var fragmentView = inflater.Inflate(Resource.Layout.HomeLayout, container, false);

            var finishedChallengesGrid = fragmentView.FindViewById<GridLayout>(Resource.Id.finishedChallengesGrid);
            var activeChallengesGrid = fragmentView.FindViewById<GridLayout>(Resource.Id.activeChallengesGrid);

            IEnumerable<Challenge> finished, active;

            using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
            {
                var myChallenges = db.Table<ChallengeProgress>()
                    .ToList()
                    .Where((c) => c.UserId == AppState.State.Instance.UserDetails.UserId);
                    

                var finishedChallengesIDs = myChallenges.Where((c) => c.Status >= ProgressStatus.Completed)
                    .Select((c) => c.ChallengeId);

                var activeChallengesIDs = myChallenges.Where((c) => c.Status == ProgressStatus.InProgress)
                    .Select((c) => c.ChallengeId);

                finished = AppState.State.Instance.Challenges
                    .Join(finishedChallengesIDs, c => c.Id, p => p, (c, _) => c);

                active = AppState.State.Instance.Challenges
                    .Join(activeChallengesIDs, c => c.Id, p => p, (c, _) => c);
            }

            PopulateGridLayoutByChallenges(finishedChallengesGrid, container.Context, finished);
            PopulateGridLayoutByChallenges(activeChallengesGrid, container.Context, active);

            var circleView = new CircleDrawView(container.Context);
            circleView.Click += (o, e) => OverviewShortcutClicked.Invoke(o, EventArgs.Empty);

            activeChallengesGrid.AddView(circleView);

            return fragmentView;
        }

        private void PopulateGridLayoutByChallenges(GridLayout grid, Context ctx, IEnumerable<Challenge> challenges)
        { 
            foreach (var challenge in challenges)
            {
                var chLayout = new LinearLayout(ctx)
                {
                    Orientation = Orientation.Vertical
                };

                var imageView = new ImageView(ctx);
                var challengeImage = BitmapFactory.DecodeFile(challenge.ImageUri.LocalPath);
                imageView.SetImageBitmap(challengeImage);

                var textView = new TextView(ctx)
                {
                    Gravity = GravityFlags.Center,
                    Text = challenge.Names[0].Name.Replace(' ', '\n')
                };

                chLayout.AddView(imageView);
                chLayout.AddView(textView);

                chLayout.Click += (sender, args) =>
                {
                    ChallengeSelected.Invoke(this, AppState.State.Instance.Challenges.ToList().IndexOf(challenge));
                };

                grid.AddView(chLayout);
            }
        }
    }
}