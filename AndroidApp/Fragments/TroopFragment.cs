using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Models.DTO;
using OdborkyApp.Base;

namespace OdborkyApp.Fragments
{
    public class TroopFragment : TitledFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        { 
            base.OnCreate(savedInstanceState);
            Title = "ODDÍL";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var fragmentView = inflater.Inflate(Resource.Layout.TroopLayout, container, false);
            var membersGrid = fragmentView.FindViewById<GridLayout>(Resource.Id.troopFragmentGrid);

            PopulateGridLayoutWithTroopProgress(membersGrid, container.Context);

            return fragmentView;
        }

        private void PopulateGridLayoutWithTroopProgress(GridLayout grid, Context ctx)
        {
            using(var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
            {
                var users = db.Table<User>().ToList();
                var groupedChallengeProgress = db.Table<ChallengeProgress>().ToList().GroupBy(c => c.UserId);
                foreach (var user in users)
                {
                    var memberProgresses = groupedChallengeProgress.Single((g) => g.Key == user.Id).ToList();
                    var memberName = new TextView(ctx)
                    {
                        Text = user.Name,
                    };
                    var activeChallenges = new TextView(ctx)
                    {
                        Text = memberProgresses.Count((p) => p.Status >= Models.ProgressStatus.Completed).ToString(),
                        
                    };
                    activeChallenges.SetPadding(20, 0, 20, 0);

                    var finishedChallenges = new TextView(ctx)
                    {
                        Text = memberProgresses.Count((p) => p.Status == Models.ProgressStatus.InProgress).ToString(),
                    };
                    finishedChallenges.SetPadding(20, 0, 20, 0);

                    grid.AddView(memberName);
                    grid.AddView(activeChallenges);
                    grid.AddView(finishedChallenges);
                }
            }

            
        }
    }
}