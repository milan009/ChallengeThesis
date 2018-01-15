using ServerApi.OwinMiddleware.Authentication;
using ServerApi.Services.Sessions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerApi.Controllers
{
    [Authorize(Roles = nameof(Roles.ChallengeAdmin))]
    [RoutePrefix("api/admins")]
    public class AdminsController : ApiController
    {
        private readonly CreateNewAdminSessionService _createSessionService = new CreateNewAdminSessionService();

        [HttpPost]
        [Route("sessions")]
        public async Task<IHttpActionResult> CreateSession()
        {
            var user = this.User as ClaimsPrincipal;
            var adminId = int.Parse(user.FindFirst("AdminId").Value);

            var sessionGuid = await _createSessionService.CreateNewAdminSessionAsync(adminId, TimeSpan.FromMinutes(30));

            return Ok(sessionGuid);
        }
    }
}
