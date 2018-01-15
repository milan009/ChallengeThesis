using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using OdborkyApp.Base;
using OdborkyApp.Fragments;
using OdborkyApp.Model;

namespace OdborkyApp.Activities
{
    [Activity(Label = "Home") ]
    public class HomeActivity : AppCompatActivity
	{
		DrawerLayout _drawerLayout;

	    readonly HomeFragment _homeFragment = new HomeFragment();
	    readonly ChallengesOverviewFragment _challengesFragment = new ChallengesOverviewFragment();
	    private readonly TroopFragment _troopFragment = new TroopFragment();

        protected override void OnCreate(Bundle bundle)
		{			
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.MainLayout);
			_drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
   

            if (!AppState.State.Instance.UserDetails.IsUnitAdmin)
		        navigationView.Menu.RemoveItem(Resource.Id.nav_scanRequest);

			var drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout, toolbar, 0, 0);
			_drawerLayout.SetDrawerListener(drawerToggle);
			drawerToggle.SyncState();

            SupportFragmentManager.BackStackChanged += (o, e) =>
            {
                var fragment = (TitledFragment)SupportFragmentManager.Fragments.FirstOrDefault((f) => f.IsAdded);
                toolbar.Title = fragment.Title;
            };

		    _challengesFragment.ChallengeSelected += (sender, i) =>
		    {
                var challangeFragment = new SingleChallengeFragment(i);

                SupportFragmentManager
                    .BeginTransaction()
		            .Replace(Resource.Id.frameLayout1, challangeFragment)
		            .AddToBackStack(null)
                    .Commit();
		    };

            _homeFragment.ChallengeSelected += (sender, i) =>
            {
                var challangeFragment = new SingleChallengeFragment(i);

                SupportFragmentManager
                    .BeginTransaction()
                    .Replace(Resource.Id.frameLayout1, challangeFragment)
                    .AddToBackStack(null)
                    .Commit();
            };

            _homeFragment.OverviewShortcutClicked += (sender, e) =>
            {
                SupportFragmentManager
                    .BeginTransaction()
                    .Replace(Resource.Id.frameLayout1, _challengesFragment)
                    .AddToBackStack(null)
                    .Commit();
            };

            SupportFragmentManager
                .BeginTransaction()
		        .Add(Resource.Id.frameLayout1, _homeFragment)
		        .Commit();
		}

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Moje zkoušky";
        }

	    private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
	    {
	        var transaction = SupportFragmentManager.BeginTransaction();
	        switch (e.MenuItem.ItemId)
	        {
	            case Resource.Id.nav_home:
	                transaction.Replace(Resource.Id.frameLayout1, _homeFragment);
	                break;

	            case Resource.Id.nav_challenges:
	                transaction.Replace(Resource.Id.frameLayout1, _challengesFragment);
	                break;

	            case Resource.Id.nav_scanRequest:
	            {
	                var qrActivityIntent = new Intent(this, typeof(QrActivity));
	                qrActivityIntent.PutExtra("scanEnum", (int) ScannerRequestEnum.ScanRequest);
	                StartActivity(qrActivityIntent);
	                return;
	            }

	            case Resource.Id.nav_snycManual:
	            {
	                var syncIntent = new Intent(this, typeof(SyncDataActivity));
	                StartActivityForResult(syncIntent, 2);
	                return;
	            }

	            case Resource.Id.nav_troop:
	                transaction.Replace(Resource.Id.frameLayout1, _troopFragment);
	                break;
	        }

	        transaction.AddToBackStack(null);
	        transaction.Commit();

	        _drawerLayout.CloseDrawers();
	    }
	}
}