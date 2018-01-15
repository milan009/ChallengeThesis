using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApi.Services.Admins
{
    public class AuthenticateAdminService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<AdminAutenticationResult> AuthenticateAdminByPassword(string username, string password)
        {
            var matchingAdmin = (await _db.Admins.Where((admin) => admin.Username == username)
                    .ToListAsync())
                .FirstOrDefault();
            if(matchingAdmin == null)
            {
                return AdminAutenticationResult.Failed;
            }

            if (BCrypt.Net.BCrypt.Verify(password, matchingAdmin.PasswordHash))
            {
                return new AdminAutenticationResult { Id = matchingAdmin.Id, Success = true };
            }

            return AdminAutenticationResult.Failed;
        }
    }

    public class AdminAutenticationResult
    {
        public bool Success { get; set; }
        public int Id { get; set; }

        public static AdminAutenticationResult Failed =
            new AdminAutenticationResult { Success = false, Id = -1 };
    }
}
