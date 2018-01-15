using System.Web.Http;
using Microsoft.Owin;
using Owin;
using ServerApi.OwinMiddleware.Authentication;

[assembly: OwinStartup(typeof(ServerApi.Startup))]

namespace ServerApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            
            app.Use<DeviceGuidAuthenticationMiddleware>(new DeviceGuidTokenAuthenticationOptions());
            app.Use<AdminGuidAuthenticationMiddleware>(new AdminGuidTokenAuthenticationOptions());
            app.Use<SkautIsAuthenticationMiddleware>(new SkautIsAuthenticationOptions());
            app.UseBasicAuthentication(new AdminBasicAuthenticationOptions());
            app.UseWebApi(config);
        }

    }
}
