using ServerApi.Services.Tasks;
using System.Threading.Tasks;
using Models.EFDB;

namespace ServerApi.Services.Challenges
{
    public class StoreChallengeService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        private readonly UpdateTaskService _updateTaskService = new UpdateTaskService();
        private readonly CreateNewTaskService _createNewTaskService = new CreateNewTaskService();

        public async Task<Challenge> UpdateChallengeAsync(int id, Challenge newChallenge)
        {
            var challenge = await _db.Challenges.FindAsync(id);

            if (challenge != null)
            {
                _db.Entry(challenge).CurrentValues.SetValues(new
                {
                    newChallenge.BasicRequirements,
                    newChallenge.CategoryId,
                    newChallenge.Description,
                    newChallenge.ExtraRequirements,
                    newChallenge.FemaleName,
                    newChallenge.MaleName,
                    newChallenge.LastModified,
                    newChallenge.ImageUri
                });
            }
            else
            {
                _db.Challenges.Add(newChallenge);
            }

            await _db.SaveChangesAsync();

            foreach (var task in newChallenge.Tasks)
            {
                if(task.Id < 0)
                {
                    await _createNewTaskService.CreateTaskAsync(task, newChallenge.Id);
                }
                else
                {
                    await _updateTaskService.UpdateTask(task);
                }
            }

            return newChallenge;
        }

        public async Task<Challenge> InsertChallengeAsync(Challenge newChallenge)
        {
            _db.Challenges.Add(newChallenge);
            await _db.SaveChangesAsync();

            foreach (var task in newChallenge.Tasks)
            {
                await _createNewTaskService.CreateTaskAsync(task, newChallenge.Id);
            //    await _updateTaskService.UpdateTask(task);
            }

            return newChallenge;
        }
    }
}