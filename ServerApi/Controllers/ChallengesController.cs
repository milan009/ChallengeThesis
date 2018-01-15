using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using ServerApi.Services.Challenges;
using System.Threading.Tasks;
using Models.EFDB;
using ServerApi.OwinMiddleware.Authentication;

namespace ServerApi.Controllers
{
    [RoutePrefix("api/challenges")]
    [Authorize(Roles = nameof(Roles.ChallengeAdmin))]
    public class ChallengesController : ApiController
    {
        private readonly GetChallengesService _getChallengesService = new GetChallengesService();
        private readonly StoreChallengeService _storeChallengeService = new StoreChallengeService();

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetChallengesAsync([FromUri] long? lastUpdate = null)
        {
            IEnumerable<Challenge> challenges;

            if (lastUpdate == null)
            {
                challenges = await _getChallengesService.GetChallengesAsync();
            }
            else
            {
                var from = new DateTime(lastUpdate.Value);
                challenges = await _getChallengesService.GetChallengesAsync(from);
            }

            var o = challenges.Select(c => (Models.DTO.Challenge) c);
            return Ok(o.ToArray());
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetChallengeAsync([FromUri] int id)
        {
            var challenge = await _getChallengesService.GetChallengeByIdAsync(id);
            if (challenge == null)
                return NotFound();

            var dto = (Models.DTO.Challenge) challenge;
            return Ok(dto);
        }

        [HttpPost]
        [Route("", Name = "PostChallenge")]
        public async Task<IHttpActionResult> PostChallengeAsync([FromBody] Models.DTO.Challenge challenge)
        {
            var inserted = await _storeChallengeService.InsertChallengeAsync((Models.EFDB.Challenge)challenge);

            return Created(Url.Route("PostChallenge", null) + "/" + inserted.Id, inserted);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutChallengeAsync([FromUri] int id, [FromBody] Models.DTO.Challenge challenge)
        {
            var updatedChallenge = await _storeChallengeService.UpdateChallengeAsync(id, (Models.EFDB.Challenge)challenge);

            if (updatedChallenge == null)
                return NotFound();

            return Ok((Models.DTO.Challenge)updatedChallenge);
        }
    }
}
