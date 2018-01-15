using System;
using System.Threading.Tasks;
using Models.EFDB;

namespace ServerApi.Services.Sessions
{
    public class CreateNewAdminSessionService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<Guid?> CreateNewAdminSessionAsync(int adminId, TimeSpan duration)
        {
            var expirationTime = DateTime.UtcNow + duration;
            var sessionGuid = Guid.NewGuid();

            var session = _db.Sessions.Add(new AdminSession { AdminId = adminId, Expires = expirationTime, Id = sessionGuid });
            _db.SaveChanges();

            if (session != null)
                return sessionGuid;

            return null;
        }
    }
}
