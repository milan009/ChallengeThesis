using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Models.DTO;
using ServerApi.Services.SkautIS;

[assembly: OwinStartup(typeof(ServerApi.Startup))]

// https://dotnetcodr.com/2015/11/23/wiring-up-a-custom-authentication-method-with-owin-in-web-api-part-3-the-components/

namespace ServerApi.OwinMiddleware.Authentication
{
    public class SkautIsAuthenticationHandler : AuthenticationHandler<SkautIsAuthenticationOptions>
    {
        private readonly SkautIsUserService _skautIsUserService = new SkautIsUserService();

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var authResult = await Authenticate(Request.Headers);
            if (!authResult.Success)
            {
                return null;
            }

            var authProps = new AuthenticationProperties();

            authProps.IssuedUtc = DateTimeOffset.UtcNow;
            authProps.ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1);
            authProps.AllowRefresh = true;
            authProps.IsPersistent = true;

            var claims = new List<Claim>
            {
                new Claim("UserId", authResult.UserDetails.UserId.ToString(), ClaimValueTypes.Integer, "SkautIs"),
                new Claim("UnitId", authResult.UserDetails.UnitId.ToString(), ClaimValueTypes.Integer, "SkautIs"),
                new Claim("PersonId", authResult.UserDetails.PersonId.ToString(), ClaimValueTypes.Integer, "SkautIs"),
                new Claim(ClaimTypes.Name, authResult.UserDetails.UserName, ClaimValueTypes.String, "SkautIs"),
                new Claim(ClaimTypes.Role, nameof(Roles.SkautIsUser), ClaimValueTypes.String, "SkautIs"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "SkautISAuth");
            
            return new AuthenticationTicket(claimsIdentity, authProps);
        }

        private async Task<AuthenticationResult> Authenticate(IHeaderDictionary requestHeaders)
        {
            // Check if request has SkautISLoginID header
            if (!requestHeaders.ContainsKey("SkautISLoginID"))
            {
                return AuthenticationResult.Fail;
            }

            // Check if SkautISLoginID has correct format
            if (!Guid.TryParse(requestHeaders.Get("SkautISLoginID"), out var skautIsLoginId))
            {
                return AuthenticationResult.Fail;
            }

            var userDetails = await _skautIsUserService.GetUserDetailsAsync(skautIsLoginId);

            return new AuthenticationResult(true, userDetails);
        }

        private class AuthenticationResult
        {
            public bool Success { get; }
            public UserDetails UserDetails { get; }

            public static readonly AuthenticationResult Fail = new AuthenticationResult(false, null);

            public AuthenticationResult(bool success, UserDetails userDetails)
            {
                Success = success;
                UserDetails = userDetails;
            }
        }
    }
}
