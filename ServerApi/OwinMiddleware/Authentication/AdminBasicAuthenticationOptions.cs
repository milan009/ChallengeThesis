using ServerApi.Services.Admins;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin;

namespace ServerApi.OwinMiddleware.Authentication
{
    public class AdminBasicAuthenticationOptions : BasicAuthenticationOptions
    {
        public AdminBasicAuthenticationOptions() : base("AdminAuth", Authenticate) { }
        private static readonly AuthenticateAdminService AuthenticateAdminService = new AuthenticateAdminService();

        public AdminBasicAuthenticationOptions(string realm, BasicAuthenticationMiddleware.CredentialValidationFunction validationFunction) : base(realm, validationFunction)
        {

        }

        private static async Task<IEnumerable<Claim>> Authenticate(string username, string password)
        {
            var result = await AuthenticateAdminService.AuthenticateAdminByPassword(username, password);
            if (result != AdminAutenticationResult.Failed)
            {
                return new List<Claim> {
                    new Claim("Name", username),
                    new Claim("AdminId", result.Id.ToString()),
                    new Claim(ClaimTypes.Role, nameof(Roles.ChallengeAdmin), ClaimValueTypes.String, "AdminsDB")
                };
            }

            return null;
        }
    }
}