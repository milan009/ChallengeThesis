using System;
using System.Xml.Serialization;

namespace Models.DTO
{
    [Serializable, XmlRoot("User")]
    public class User : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool UnitAdmin { get; set; }

        public MembershipCategoryId Category { get; set; }
        public byte[] PublicKeyEncoded { get; set; }

        public int UnitId { get; set; }
    }
}