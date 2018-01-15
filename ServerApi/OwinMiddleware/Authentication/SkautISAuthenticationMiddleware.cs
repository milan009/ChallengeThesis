using Microsoft.Owin.Security.Infrastructure;

namespace ServerApi.OwinMiddleware.Authentication
{
    public class SkautIsAuthenticationMiddleware : AuthenticationMiddleware<SkautIsAuthenticationOptions>
    {
        public SkautIsAuthenticationMiddleware(
            Microsoft.Owin.OwinMiddleware next,
            SkautIsAuthenticationOptions options) : base(next, options)
        {

        }

        protected override AuthenticationHandler<SkautIsAuthenticationOptions> CreateHandler()
        {
            return new SkautIsAuthenticationHandler();
        }
    }
}