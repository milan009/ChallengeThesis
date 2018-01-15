using System;
using Android.App;
using Android.Content;
using Android.OS;
using Models.DTO;
using Newtonsoft.Json;

namespace OdborkyApp.Base
{
    [Activity(Label = "AccountRequiringBaseActivity")]
    public abstract class AccountRequiringBaseActivity<TRegisterActivity> : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (AppState.State.Instance.UserDetails == null)
            {
                var sharedPrefs = GetSharedPreferences("ChallengeApp", FileCreationMode.Private);

                var userDetailsJson = sharedPrefs.GetString("userDetailsJson", null);

                if (userDetailsJson == null)
                {
                    StartActivity(typeof(TRegisterActivity));
                    Finish();
                    return;
                }

                AppState.State.Instance.UserDetails = JsonConvert.DeserializeObject<Model.UserDetails>(userDetailsJson);
                var kc = new KeyChain.Net.XamarinAndroid.KeyChainHelper(() => this);

                if (Guid.TryParse(kc.GetKey("deviceGuid"), out var guid))
                {
                    AppState.State.Instance.DeviceGuid = guid;
                }
                else
                {
                    StartActivity(typeof(TRegisterActivity));
                    Finish();
                    return;
                }
            }

            SetupDb();
            OnUserDetailsLoaded();
        }

        protected abstract void OnUserDetailsLoaded();

        private void SetupDb()
        {
            using (var db = new SQLite.SQLiteConnection(AppState.State.Instance.DbPath))
            {
                db.CreateTable<User>();
                db.CreateTable<Model.ChallengeProgress>();
                db.CreateTable<Model.TaskProgress>();
            }
        }
    }
}