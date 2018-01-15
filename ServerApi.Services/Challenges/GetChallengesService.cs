using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Models.EFDB;

namespace ServerApi.Services.Challenges
{
    public class GetChallengesService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<IEnumerable<Challenge>> GetChallengesAsync()
        {
            return await _db.Challenges.Include(c => c.Tasks).ToListAsync();
        }

        public async Task<IEnumerable<Challenge>> GetChallengesAsync(DateTime lastTimestamp)
        {
            return await _db.Challenges.Include(c => c.Tasks).Where(c => c.LastModified > lastTimestamp).ToListAsync();
        }

        public async Task<Challenge> GetChallengeByIdAsync(int challengeId)
        {
            return await _db.Challenges.Include(c => c.Tasks).Where(c => c.Id == challengeId).FirstOrDefaultAsync();
        }
    }
}