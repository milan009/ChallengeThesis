using System.Threading.Tasks;
using Models.EFDB;

namespace ServerApi.Services.Tasks
{
    public class UpdateTaskService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<ChallengeTask> UpdateTask(ChallengeTask newTask)
        {
            var task = await _db.ChallengesTasks.FindAsync(newTask.Id);

            _db.Entry(task).CurrentValues.SetValues(new
            {
                newTask.ChallengeId,
                newTask.Competences,
                newTask.Description,
                newTask.Extra,
                newTask.Name
            });
            await _db.SaveChangesAsync();

            return task;
        }
    }
}
