using Microsoft.Owin.Security;

namespace ServerApi.OwinMiddleware.Authentication
{
    public class AdminGuidTokenAuthenticationOptions : AuthenticationOptions
    {
        public AdminGuidTokenAuthenticationOptions() : base("AdminGuidTokenAuth")
        {
            
        }
    }
}