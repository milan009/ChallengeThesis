using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Models.EFDB;
using ServerApi.Services.Sessions;

[assembly: OwinStartup(typeof(ServerApi.Startup))]

// Followed this tutorial
// https://dotnetcodr.com/2015/11/23/wiring-up-a-custom-authentication-method-with-owin-in-web-api-part-3-the-components/
// https://msdn.microsoft.com/en-us/library/ff359101.aspx

namespace ServerApi.OwinMiddleware.Authentication
{
    public class AdminGuidAuthenticationHandler : AuthenticationHandler<AdminGuidTokenAuthenticationOptions>
    {
        private readonly GetAdminSessionService _getAdminSessionService = new GetAdminSessionService();

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var authResult = await Authenticate(Request.Headers);
            if (!authResult.Success)
            {
                return null;
            }

            var authProps = new AuthenticationProperties();

            authProps.IssuedUtc = DateTimeOffset.UtcNow;
            authProps.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30);
            authProps.AllowRefresh = true;
            authProps.IsPersistent = true;

            var claims = new List<Claim>
            {
             //   new Claim("AdminId", authResult.UserId.ToString(), ClaimValueTypes.Integer, "RegisteredUsersDB"),
                new Claim("AdminId", Request.Headers["AdminGuid"], "Guid", "Client"),
                new Claim(ClaimTypes.Role, nameof(Roles.ChallengeAdmin), ClaimValueTypes.String, "AdminUsersDB"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "DeviceGuidTokenAuth");
            return new AuthenticationTicket(claimsIdentity, authProps);
        }

        private async Task<AuthenticationResult> Authenticate(IHeaderDictionary requestHeaders)
        {
            // Check if request has DeviceGuid header
            if (!requestHeaders.ContainsKey("AdminGuid"))
            {
                return AuthenticationResult.Fail;
            }

            // Check if DeviceGuid has correct format
            if (Guid.TryParse(requestHeaders.Get("AdminGuid"), out var adminGuid))
            {
                Request.Set("AdminGuid", adminGuid);
            }
            else
            {
                return AuthenticationResult.Fail;
            }

            // Check if this device GUID is registered to a user and retrieve that userId
            var admin = await _getAdminSessionService.GetAdminBySessionIdAsync(adminGuid);
            if (admin == null)
            {
                return AuthenticationResult.Fail;
            }

            // Request.Set("UserId", userId.Value);
            return new AuthenticationResult(true, admin);
        }

        private class AuthenticationResult
        {
            public bool Success { get; }
            public AdminSession Admin { get; }

            public static readonly AuthenticationResult Fail = new AuthenticationResult(false, null);

            public AuthenticationResult(bool success, AdminSession admin)
            {
                Success = success;
                Admin = Admin;
            }
        }
    }
}
