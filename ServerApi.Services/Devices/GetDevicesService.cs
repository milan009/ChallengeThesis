using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApi.Services.Devices
{
    public class GetDevicesService
    {
        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<int?> GetUserIdByDeviceIdAsync(Guid deviceGuid)
        {
            var users = await _db.Devices.Where(device => device.Id == deviceGuid).Select(device => device.UserId).ToListAsync();

            if (users.Any())
            {
                return users.First();
            }

            return null;
        }
    }
}
