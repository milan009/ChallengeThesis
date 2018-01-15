using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Models.EFDB;
using ServerApi.Services.Devices;

namespace ServerApi.Services.Users
{
    public class GetUsersService
    {
        private readonly ServerApiContext _db = new ServerApiContext();
        private readonly GetDevicesService _getDevicesService = new GetDevicesService();

        public async Task<User> GetUserByDeviceAsync(Guid deviceGuid)
        {
            var userId = await _getDevicesService.GetUserIdByDeviceIdAsync(deviceGuid);
            return await _db.Users.FindAsync(userId);
        }

        public async Task<IEnumerable<User>> GetUsersOfUnitAsync(Guid deviceGuid, DateTime lastTimestamp)
        {
            var deviceUser = await GetUserByDeviceAsync(deviceGuid);
            if (deviceUser == null)
            {
                return null;
            }

            return _db.Users.Where(user => user.UnitId == deviceUser.UnitId && user.LastModified > lastTimestamp);
        }

        public async Task<IEnumerable<User>> GetUsersOfUnitAsync(Guid deviceGuid)
        {
            var deviceUser = await GetUserByDeviceAsync(deviceGuid);
            if (deviceUser == null)
            {
                return null;
            }

            return _db.Users.Where(user => user.UnitId == deviceUser.UnitId);
        }

        public async Task<User> GetUserByIdAsync(Guid deviceGuid, int userId)
        {
            var deviceUser = await GetUserByDeviceAsync(deviceGuid);
            if (deviceUser == null)
            {
                return null;
            }

            return await _db.Users.Where(user => user.UnitId == deviceUser.UnitId && user.Id == userId).FirstOrDefaultAsync();
        }
    }
}