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
    [RoutePrefix("api/progress/tasks")]
    public class TaskProgressController : ApiController
    {
        private readonly TasksProgressService _tasksProgressService = new TasksProgressService();
        private readonly GetUsersService _getUsersService = new GetUsersService();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetTaskProgresses([FromUri] long? lastUpdate = null)
        {
            var claimUser = this.User as ClaimsPrincipal;
            var deviceGuid = Guid.Parse(claimUser.FindFirst("DeviceId").Value);
            var unitId = (await _getUsersService.GetUserByDeviceAsync(deviceGuid)).UnitId;

            IEnumerable<TaskProgress> tProgresses;

            if (lastUpdate == null)
            {
                tProgresses = await _tasksProgressService.GetTasksProgressesByUnitIdAsync(unitId);
            }
            else
            {
                var from = new DateTime(lastUpdate.Value);
                tProgresses = await _tasksProgressService.GetTasksProgressesByUnitIdAsync(unitId, from);
            }

            var DTOs = tProgresses.Select(tp => (Models.DTO.TaskProgress)tp);

            return Ok(DTOs);
        }

        [HttpPut]
        [Route("{taskProgressId}", Name = "PutTaskProgress")]
        public async Task<IHttpActionResult> PutTaskProgress([FromUri] Guid taskProgressId, [FromBody] Models.DTO.TaskProgress taskProgress)
        {
            await _tasksProgressService.StoreTaskProgressAsync(taskProgress);

            return Created(Url.Route("PutTaskProgress", null), taskProgress);
        }
    }
}
