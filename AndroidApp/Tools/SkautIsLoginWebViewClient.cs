using System;
using System.Linq;
using System.Text.RegularExpressions;
using Android.Graphics;
using Android.Webkit;

namespace OdborkyApp.Tools
{
    internal class SkautIsLoginWebViewClient : WebViewClient, IValueCallback
    {
        public EventHandler<string[]> OnParseCredentials;

        private bool _extractedCredentials;
        private string[] _credentials;

        public SkautIsLoginWebViewClient(WebView view)
        {
            view.Settings.JavaScriptEnabled = true;
            view.SetWebViewClient(this);
            view.LoadUrl("https://is.skaut.cz/Login/?appid=6C073215-1D9A-4FCB-866F-DAC4B69110DB&ReturnUrl=%2fJunak%2f");
        }

        // Get login guid from login page

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            if (url.Contains("LoggedIn"))
            {
                view.EvaluateJavascript(
                    "(function() { return (document.getElementsByTagName('form')[0].innerHTML); })();", this);
            }

            if (_extractedCredentials)
            {
                base.OnPageStarted(view, url, favicon);
                OnParseCredentials.Invoke(this, _credentials);
                _extractedCredentials = false;
            }
        }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            Regex r = new Regex(@"(?<=value=\\"").*?(?=\\)");
            var mat = r.Matches((string) value);
            _credentials = mat.OfType<Match>().Select(m => m.Value).ToArray();
            _extractedCredentials = true;
        }
    }
}