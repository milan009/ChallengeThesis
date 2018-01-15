using Microsoft.Owin.Security.Infrastructure;

namespace ServerApi.OwinMiddleware.Authentication
{
    public class DeviceGuidAuthenticationMiddleware : AuthenticationMiddleware<DeviceGuidTokenAuthenticationOptions>
    {
        public DeviceGuidAuthenticationMiddleware(
            Microsoft.Owin.OwinMiddleware next,
            DeviceGuidTokenAuthenticationOptions options) : base(next, options)
        {

        }

        protected override AuthenticationHandler<DeviceGuidTokenAuthenticationOptions> CreateHandler()
        {
            return new DeviceGuidAuthenticationHandler();
        }
    }
}