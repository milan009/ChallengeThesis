using System;
using System.IO;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Newtonsoft.Json;
using OdborkyApp.Model;
using OdborkyApp.Tools;
using Plugin.Connectivity;

namespace OdborkyApp.Activities
{
    [Activity(Label = "AndroidApp", Icon = "@drawable/icon")]
    public class SkautIsConnectActivity : Activity
    {
        private string _loginId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (!CrossConnectivity.Current.IsConnected)
            {
                new AlertDialog
                        .Builder(this)
                    .SetMessage("První spuštění aplikace vyžaduje připojení k internetu.")
                    .SetNeutralButton("Ok", (o, e) => { FinishAndRemoveTask(); })
                    .Create()
                    .Show();
            }
            else
            {
                WebView v = new WebView(this);
                var c = new SkautIsLoginWebViewClient(v);
                c.OnParseCredentials += StoreLoginId;

                SetContentView(v);
            }
        }

        private void OnLoggedIn()
        {
            UserDetails userDetails = null;
            string responseJson;

            var request = new HttpWebRequest(new Uri("https://challengethesis.azurewebsites.net/api/users"));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("SkautISLoginID", _loginId);

            try
            {
                using (var stream = request.GetRequestStream())
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write("");
                }
            }
            catch (WebException e)
            {
                var l = e;
            }
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream))
            {
                responseJson = streamReader.ReadToEnd();
            }

            var responseObject = JsonConvert.DeserializeObject<ParsedRegisterResponse>(responseJson);
            userDetails = responseObject.Item1;

            using (var sharedPrefs = GetSharedPreferences("ChallengeApp", FileCreationMode.Private))
            using (var editor = sharedPrefs.Edit())
            {
                var userDetailsJson = JsonConvert.SerializeObject(userDetails);

                editor.PutString(nameof(userDetailsJson), userDetailsJson);
                editor.Commit();
            }
               
            var kc = new KeyChain.Net.XamarinAndroid.KeyChainHelper(() => this);
            kc.SetKey("deviceGuid", responseObject.Item2.ToString());
            kc.SetKey("privateKey", responseObject.Item3);

            StartActivity(typeof(WelcomeActivity));
            SetResult(Result.Ok);
            Finish();
        }

        private void StoreLoginId(object sender, string[] creds)
        {
            _loginId = creds[0];

            SetContentView(new View(this));
            OnLoggedIn();
        }
    }
}