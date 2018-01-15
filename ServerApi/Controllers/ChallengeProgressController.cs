using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Models.EFDB;
using ServerApi.OwinMiddleware.Authentication;
using ServerApi.Services.Progress;
using ServerApi.Services.Users;

namespace ServerApi.Controllers
{
    [Authorize(Roles = nameof(Roles.RegisteredUser))]
    [RoutePrefix("api/progress/challenges")]
    public class ChallengesProgressController : ApiController
    {
        private readonly ChallengesProgressService _challengesProgressService = new ChallengesProgressService();
        private readonly GetUsersService _getUsersService = new GetUsersService();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetChallengeProgresses([FromUri] long? lastUpdate = null)
        {
            var claimUser = this.User as ClaimsPrincipal;
            var deviceGuid = Guid.Parse(claimUser.FindFirst("DeviceId").Value);
            var unitId = (await _getUsersService.GetUserByDeviceAsync(deviceGuid)).UnitId;

            IEnumerable<ChallengeProgress> cProgresses;

            if (lastUpdate == null)
            {
                cProgresses = await _challengesProgressService.GetChallengeProgressesByUnitIdAsync(unitId);
            }
            else
            {
                var from = new DateTime(lastUpdate.Value);
                cProgresses = await _challengesProgressService.GetChallengeProgressesByUnitIdAsync(unitId, from);
            }

            var DTOs = cProgresses.Select(tp => (Models.DTO.ChallengeProgress)tp);

            return Ok(DTOs);
        }

        [HttpPut]
        [Route("{challengeProgressId}", Name = "PutChallengeProgress")]
        public async Task<IHttpActionResult> PutChallengeProgress([FromUri] Guid challengeProgressId, [FromBody] Models.DTO.ChallengeProgress challengeProgress)
        {
            var claimUser = this.User as ClaimsPrincipal;
            var deviceGuid = Guid.Parse(claimUser.FindFirst("DeviceId").Value);

            if (challengeProgressId != challengeProgress.Id)
            {
                return BadRequest();
            }

            if (!await _challengesProgressService.IsAuthorizedToStoreProgress(challengeProgress, deviceGuid))
            {
                return Unauthorized(null);
            }

            await _challengesProgressService.StoreChallengeProgressAsync(challengeProgress);

            return Created(Url.Route("PutChallengeProgress", null), challengeProgress);
        }
    }
}
