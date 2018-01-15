using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using ServerApi.Services.Devices;

[assembly: OwinStartup(typeof(ServerApi.Startup))]

// https://dotnetcodr.com/2015/11/23/wiring-up-a-custom-authentication-method-with-owin-in-web-api-part-3-the-components/
// https://msdn.microsoft.com/en-us/library/ff359101.aspx
namespace ServerApi.OwinMiddleware.Authentication
{
    public class DeviceGuidAuthenticationHandler : AuthenticationHandler<DeviceGuidTokenAuthenticationOptions>
    {
        private readonly GetDevicesService _getDevicesService = new GetDevicesService();

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
                new Claim("UserId", authResult.UserId.ToString(), ClaimValueTypes.Integer, "RegisteredUsersDB"),
                new Claim("DeviceId", Request.Headers["DeviceGuid"], "Guid", "Client"),
                new Claim(ClaimTypes.Role, nameof(Roles.RegisteredUser), ClaimValueTypes.String, "RegisteredUsersDB"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "DeviceGuidTokenAuth");
            return new AuthenticationTicket(claimsIdentity, authProps);
        }

        private async Task<AuthenticationResult> Authenticate(IHeaderDictionary requestHeaders)
        {
            // Check if request has DeviceGuid header
            if (!requestHeaders.ContainsKey("DeviceGuid"))
            {
                return AuthenticationResult.Fail;
            }

            // Check if DeviceGuid has correct format
            if (Guid.TryParse(requestHeaders.Get("DeviceGuid"), out var deviceGuid))
            {
                Request.Set("DeviceGuid", deviceGuid);
            }
            else
            {
                return AuthenticationResult.Fail;
            }

            // Check if this device GUID is registered to a user and retrieve that userId
            int? userId = await _getDevicesService.GetUserIdByDeviceIdAsync(deviceGuid);
            if (userId == null)
            {
                return AuthenticationResult.Fail;
            }

            // Request.Set("UserId", userId.Value);
            return new AuthenticationResult(true, userId.Value);
        }

        private class AuthenticationResult
        {
            public bool Success { get; }
            public int UserId { get; }

            public static readonly AuthenticationResult Fail = new AuthenticationResult(false, -1);

            public AuthenticationResult(bool success, int userId)
            {
                Success = success;
                UserId = userId;
            }
        }
    }
}
