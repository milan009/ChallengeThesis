using System;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Models.DTO;
using OdborkyApp.Base;

namespace OdborkyApp.Fragments
{
    public class ChallengesOverviewFragment : TitledFragment
    {
        private readonly string[] _categoryNames = 
        {
            "UMĚLECKÉ",
            "TECHNICKÉ",
            "TÁBORNICKO-CESTOVATELSKÉ",
            "HUMANITNÍ",
            "PŘÍRODOVĚDECKÉ",
            "VODÁCKÉ",
            "SLUŽBA BLIŽNÍM",
            "SPORTOVNÍ",
            "DUCHOVNÍ",
            "ŽIVOT V ODDÍLE",
        };

        public event EventHandler<int> ChallengeSelected;

        private ChallengeCategoryEnum _selectedCategory;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = "Odborky";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var fragmentView = inflater.Inflate(Resource.Layout.ChallengesOverviewLayout, container, false);
            var challengesGrid = fragmentView.FindViewById<GridLayout>(Resource.Id.challengesOverviewGrid);

            var nextChallengeBtn = fragmentView.FindViewById<Button>(Resource.Id.nextChallengeCategory);
            var prevChallengeBtn = fragmentView.FindViewById<Button>(Resource.Id.prevChallengeCategory);
            var challengeCategoryText = fragmentView.FindViewById<TextView>(Resource.Id.challengesOverviewText);

            nextChallengeBtn.Click += (o, e) =>
            {
                _selectedCategory = Enum.GetValues(typeof(ChallengeCategoryEnum)).Cast<ChallengeCategoryEnum>()
                    .FirstOrDefault((c) => (int)c > (int)_selectedCategory);

                PopulateGridLayoutByCategory(challengesGrid, container.Context, _selectedCategory);
                challengeCategoryText.Text = _categoryNames[(int)_selectedCategory];
            };

            prevChallengeBtn.Click += (o, e) =>
            {
                _selectedCategory = _selectedCategory == ChallengeCategoryEnum.Art ? ChallengeCategoryEnum.Scouting : 
                Enum.GetValues(typeof(ChallengeCategoryEnum)).Cast<ChallengeCategoryEnum>()
                    .LastOrDefault((c) => (int)c < (int)_selectedCategory);

                PopulateGridLayoutByCategory(challengesGrid, container.Context, _selectedCategory);
                challengeCategoryText.Text = _categoryNames[(int)_selectedCategory];
            };

            challengesGrid.ColumnCount = 3;

            PopulateGridLayoutByCategory(challengesGrid, container.Context, _selectedCategory);
            challengeCategoryText.Text = _categoryNames[(int)_selectedCategory];

            return fragmentView;
        }

        private void PopulateGridLayoutByCategory(GridLayout grid, Context ctx, ChallengeCategoryEnum category)
        {
            var challenges = AppState.State.Instance.Challenges.Where(c => c.Category == category);

            grid.RemoveAllViews();

            foreach(var challenge in challenges)
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