using Android.App;
using Android.Content;
using Android.OS;
using OdborkyApp.Activities;

namespace OdborkyApp.Base
{
    [Activity(Label = "ChallangeAppBaseActivity")]
    public abstract class LoadedChallengesBasedActivity : AccountRequiringBaseActivity<SkautIsConnectActivity>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (AppState.State.Instance.Challenges != null)
            {
                OnChallengesLoaded();
            }
            else
            {
                StartActivityForResult(typeof(SyncDataActivity), 0);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
                OnChallengesLoaded();
        }

        protected sealed override void OnUserDetailsLoaded() { }

        protected abstract void OnChallengesLoaded();
    }
}