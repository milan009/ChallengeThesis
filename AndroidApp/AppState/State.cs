using System.Collections.Generic;
using System.IO;
using Models.DTO;
using UserDetails = OdborkyApp.Model.UserDetails;

namespace OdborkyApp.AppState
{
    public class State
    {
        private State()
        {
            var dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var chFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var dbFileName = "db";

            ChallengesPath = Path.Combine(chFolder, "challenges");
            DbPath = Path.Combine(dbFolder, dbFileName);
        }

        public static State Instance { get; } = new State();
 
        public UserDetails UserDetails { get; set; }
        public IEnumerable<Challenge> Challenges { get; set; }

        public string DbPath { get; set; }
        public string ChallengesPath { get; set; }
        public System.Guid DeviceGuid { get; set; }
    }
}