using Microsoft.Owin.Security.Infrastructure;

namespace ServerApi.OwinMiddleware.Authentication
{
    public class AdminGuidAuthenticationMiddleware : AuthenticationMiddleware<AdminGuidTokenAuthenticationOptions>
    {
        public AdminGuidAuthenticationMiddleware(
            Microsoft.Owin.OwinMiddleware next,
            AdminGuidTokenAuthenticationOptions options) : base(next, options)
        {

        }

        protected override AuthenticationHandler<AdminGuidTokenAuthenticationOptions> CreateHandler()
        {
            return new AdminGuidAuthenticationHandler();
        }
    }
}