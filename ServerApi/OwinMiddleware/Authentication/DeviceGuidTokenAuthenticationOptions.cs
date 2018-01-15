using Microsoft.Owin.Security;

namespace ServerApi.OwinMiddleware.Authentication
{
    public class DeviceGuidTokenAuthenticationOptions : AuthenticationOptions
    {
        public DeviceGuidTokenAuthenticationOptions() : base("DeviceGuidTokenAuth")
        {
            
        }
    }
}