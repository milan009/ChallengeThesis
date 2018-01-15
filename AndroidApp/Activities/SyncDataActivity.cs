using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using OdborkyApp.Base;
using OdborkyApp.Service;
using Android.Widget;

namespace OdborkyApp.Activities
{
    [Activity(Label = "SyncDataActivity", NoHistory = true)]
    public class SyncDataActivity : AccountRequiringBaseActivity<SkautIsConnectActivity>
    {
        protected override void OnUserDetailsLoaded()
        {
            DateTime? lastSync;
            SetContentView(Resource.Layout.SyncingLayout);

            using (var prefs = GetSharedPreferences("ChallengeApp", FileCreationMode.Private))
            {
                var lastSyncTicks = prefs.GetLong("LastSyncTicks", -1);
               
                if (lastSyncTicks < 0)
                    lastSync = null;
                else
                    lastSync = new DateTime(lastSyncTicks);
            }

            var s = new SyncService(lastSync);
            s.SyncSuccess += OnSyncSuccess;
            s.SyncFail += OnSyncFail;

            Task.Run(() => s.Synchronize());
        }

        private void OnSyncFail(object sender, Exception e)
        {
            if (typeof(NotConnectedException) == e.GetType())
            {
                StartActivity(typeof(HomeActivity));
                Finish();
            }
            else
            {
                FinishAndRemoveTask();
            }
        }

        private void OnSyncSuccess(object sender, EventArgs e)
        {
            using (var prefs = GetSharedPreferences("ChallengeApp", FileCreationMode.Private))
            using (var editor = prefs.Edit())
            {
                editor.PutLong("LastSyncTicks", DateTime.UtcNow.Ticks);
                editor.Commit();
            }

            StartActivity(typeof(HomeActivity));
            Finish();
        }
    }
}