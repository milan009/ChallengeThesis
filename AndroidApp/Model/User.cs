using Models;
using SQLite;

namespace OdborkyApp.Model
{
    internal class User : IUser
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool UnitAdmin { get; set; }

        public MembershipCategoryId Category { get; set; }
        public byte[] PublicKeyEncoded { get; set; }

        public int UnitId { get; set; }
    }
}