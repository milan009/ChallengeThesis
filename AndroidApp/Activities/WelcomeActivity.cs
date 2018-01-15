using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;

namespace OdborkyApp.Activities
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = true, NoHistory = true)]
    public class WelcomeActivity : AppCompatActivity
    {
        private const int SyncRequestCode = 4;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            bool firstTime;

            using (var prefs = GetSharedPreferences("ChallengeApp", FileCreationMode.Private))
            {
                firstTime = prefs.GetBoolean("firstTime", true);
            }

            if (firstTime)
            {
                SetContentView(Resource.Layout.WelcomeLayout);
                var btn = FindViewById<Button>(Resource.Id.welcomeBtn);

                btn.Click += (o, e) =>
                {
                    using (var prefs = GetSharedPreferences("ChallengeApp", FileCreationMode.Private))
                    using (var prefsEditor = prefs.Edit())
                    {
                        prefsEditor.PutBoolean("firstTime", false);
                        prefsEditor.Commit();

                        StartActivity(typeof(SyncDataActivity));
                        Finish();
                    }
                };
            }
            else
            {
                StartActivity(typeof(SyncDataActivity));
                Finish();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == SyncRequestCode)
            {
                StartActivity(typeof(HomeActivity));
                Finish();
            }
        }
    }
}