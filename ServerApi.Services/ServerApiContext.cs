using System.Data.Entity;
using Models.EFDB;

namespace ServerApi.Services
{
    public class ServerApiContext : DbContext
    {
        public ServerApiContext() : base("name=ServerApiContext")
        {
            Database.Initialize(true);
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminSession> Sessions { get; set; }

        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<ChallengeTask> ChallengesTasks { get; set; }

        public DbSet<ChallengeProgress> ChallengesProgresses { get; set; }
        public DbSet<TaskProgress> TasksProgresses { get; set; }
    }
}
