using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using ServerApi.Services.Users;

namespace ServerApi.Services.Progress
{
    public class ChallengesProgressService
    {
        private readonly ServerApiContext _db = new ServerApiContext();
        private readonly GetUsersService _getUsersService = new GetUsersService(); 

        public async Task<IEnumerable<Models.EFDB.ChallengeProgress>> GetChallengeProgressesByUnitIdAsync(int unitId)
        {
            return await _db.ChallengesProgresses.Where(progress => progress.User.UnitId == unitId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.EFDB.ChallengeProgress>> GetChallengeProgressesByUnitIdAsync(int unitId, DateTime lastTimestamp)
        {
            return await _db.ChallengesProgresses
                .Where(progress => progress.User.UnitId == unitId && progress.LastModified > lastTimestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.EFDB.ChallengeProgress>> GetChallengeProgressesByUserIdAsync(int userId)
        {
            return await _db.ChallengesProgresses.Where(t => t.User.Id == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.EFDB.ChallengeProgress>> StoreChallengeProgressBatchAsync(
            IEnumerable<Models.DTO.ChallengeProgress> progresses)
        {
            var efdbProgresses = progresses.Select((p) => (Models.EFDB.ChallengeProgress) p).ToArray();

            _db.ChallengesProgresses.AddOrUpdate(efdbProgresses);
            await _db.SaveChangesAsync();

            return efdbProgresses;
        }

        public async Task<Models.EFDB.ChallengeProgress> StoreChallengeProgressAsync(Models.DTO.ChallengeProgress challengeProgress)
        {
            var efdbProgress = (Models.EFDB.ChallengeProgress) challengeProgress;

            _db.ChallengesProgresses.AddOrUpdate(efdbProgress);
            await _db.SaveChangesAsync();

            return efdbProgress;
        }

        public async Task<bool> IsAuthorizedToStoreProgress(Models.DTO.ChallengeProgress challengeProgress, Guid deviceGuid)
        {
            var postingUser = await _getUsersService.GetUserByDeviceAsync(deviceGuid);
            var targetUser = await _getUsersService.GetUserByIdAsync(deviceGuid, challengeProgress.UserId);

            if (targetUser == null)
                return false;

            if (targetUser == postingUser && challengeProgress.Status <= Models.ProgressStatus.Completed)
                return true;

            return postingUser.UnitAdmin;
        }
    }
}