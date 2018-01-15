using System.Linq;
using System.Threading.Tasks;
using Models.EFDB;

namespace ServerApi.Services.Tasks
{
    public class CreateNewTaskService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<ChallengeTask> CreateTaskAsync(ChallengeTask newTask, int challengeId)
        {
            var createdTask = newTask;
            createdTask.ChallengeId = challengeId;
            createdTask.Id = _db.ChallengesTasks.Count();

            var storedTask = _db.ChallengesTasks.Add(createdTask);

            return storedTask;
        }
    }
}
