using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Models.EFDB;

namespace ServerApi.Services.Sessions
{
    public class GetAdminSessionService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<AdminSession> GetAdminBySessionIdAsync(Guid sessionGuid)
        {
            var session = await _db.Sessions.Where(s => s.Expires > DateTime.UtcNow).SingleOrDefaultAsync(s => s.Id == sessionGuid);
            return session;
        }
    }
}
