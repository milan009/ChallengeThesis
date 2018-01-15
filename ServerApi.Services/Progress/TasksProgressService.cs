using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;


namespace ServerApi.Services.Progress
{
    public class TasksProgressService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<IEnumerable<Models.EFDB.TaskProgress>> GetTasksProgressesByUnitIdAsync(int unitId)
        {
            return await _db.TasksProgresses
                .Where(t => t.ChallengeProgress.User.UnitId == unitId)
                .Include(t => t.Task)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.EFDB.TaskProgress>> GetTasksProgressesByUnitIdAsync(int unitId, DateTime lastUpdate)
        {
            return await _db.TasksProgresses
                .Where(t => t.ChallengeProgress.User.UnitId == unitId 
                    && t.LastModified >= lastUpdate)
                .Include(t => t.Task)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.EFDB.TaskProgress>> StoreTaskProgressBatchAsync(
            IEnumerable<Models.DTO.TaskProgress> progresses)
        {
            var efdbProgresses = await _db.ChallengesTasks
                .Join(progresses, (t) => t.Id, (p) => p.TaskId, (t, p) =>
                new Models.EFDB.TaskProgress
                {
                    Id = p.Id,
                    ChallengeProgressId = p.ChallengeProgressId,
                    LastModified = p.LastModified,
                    Status = p.Status,
                    Task = t
                }).ToArrayAsync();

            _db.TasksProgresses.AddOrUpdate(efdbProgresses);
            await _db.SaveChangesAsync();

            return efdbProgresses;
        }

        public async Task<Models.EFDB.TaskProgress> StoreTaskProgressAsync(Models.DTO.TaskProgress taskProgress)
        {
            var targetTask = await _db.ChallengesTasks.FirstOrDefaultAsync((t) => t.Id == taskProgress.TaskId);
            var efdbProgress = new Models.EFDB.TaskProgress
            {
                Id = taskProgress.Id,
                ChallengeProgressId = taskProgress.ChallengeProgressId,
                LastModified = taskProgress.LastModified,
                Status = taskProgress.Status,
                Task = targetTask
            };

            _db.TasksProgresses.AddOrUpdate(efdbProgress);
            await _db.SaveChangesAsync();

            return efdbProgress;
        }
    }
}
