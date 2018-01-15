using Microsoft.Owin.Security;

namespace ServerApi.OwinMiddleware.Authentication
{
    public class SkautIsAuthenticationOptions : AuthenticationOptions
    {
        public SkautIsAuthenticationOptions() : base("SkautISAuth")
        {

        }
    }
}