using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Models.DTO;
using ServerApi.OwinMiddleware.Authentication;
using ServerApi.Services.Users;

namespace ServerApi.Controllers
{
    [RoutePrefix("api/users")]
    [Authorize(Roles = nameof(Roles.RegisteredUser))]
    
    public class UsersController : ApiController
    {
        private readonly GetUsersService _getUsersService = new GetUsersService();
        private readonly RegisterUserService _registerUserService = new RegisterUserService();
        private Guid _deviceGuid;

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetUsers([FromUri] long? lastUpdate = null)
        {
            var user = this.User as ClaimsPrincipal;
            IEnumerable<Models.EFDB.User> users;

            _deviceGuid = Guid.Parse(user.FindFirst("DeviceId").Value);

            if (lastUpdate == null)
            {
                users = await _getUsersService.GetUsersOfUnitAsync(_deviceGuid);
            }
            else
            {
                var from = new DateTime(lastUpdate.Value);
                users = await _getUsersService.GetUsersOfUnitAsync(_deviceGuid, from);
            }

            var dtOs = users.Select(u => (User)u);
            return Ok(dtOs);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetUser([FromUri] int id)
        {
            _deviceGuid = Request.GetOwinContext().Get<Guid>("DeviceGuid");
            var user = await _getUsersService.GetUserByIdAsync(_deviceGuid, id);
            return Ok((User)user);
        }

        [HttpGet]
        [Route("unit/{unitId}")]
        public async Task<IHttpActionResult> GetUsersOfUnit([FromUri] int unitId)
        {
            _deviceGuid = Request.GetOwinContext().Get<Guid>("DeviceGuid");
            var users = await _getUsersService.GetUsersOfUnitAsync(_deviceGuid);

            var usersDtOs = users.Select(u => (User) u);

            return Ok(usersDtOs);
        }

        [HttpGet]
        [Route("unit/{unitId}/{ticks}")]
        public async Task<IHttpActionResult> GetUsersOfUnit([FromUri] int unitId, [FromUri] long ticks)
        {
            _deviceGuid = Request.GetOwinContext().Get<Guid>("DeviceGuid");
            var from = new DateTime(ticks);
            var users = await _getUsersService.GetUsersOfUnitAsync(_deviceGuid, from);

            var usersDtOs = users.Select(u => (User)u);

            return Ok(usersDtOs);
        }

        [HttpPost]
        [OverrideAuthorization]
        [Authorize(Roles = nameof(Roles.SkautIsUser))]
        [Route("")]
        public async Task<IHttpActionResult> PostUser()
        {
            var skautIsLoginId = Guid.Parse(Request.Headers.GetValues("SkautISLoginID").First());
            var r = await _registerUserService.RegisterUserAsync(skautIsLoginId);

            return Ok(r);
        }
    }
}
