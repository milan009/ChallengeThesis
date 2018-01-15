using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models.EFDB
{
    public class User
    {
        [Key]
        [DatabaseG‌​enerated(DatabaseGen‌​eratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool UnitAdmin { get; set; }

        public MembershipCategoryId Category { get; set; }
        public byte[] PublicKeyEncoded { get; set; }
        public byte[] PrivateKeyEncoded { get; set; }

        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        public DateTime LastModified { get; set; }

        public static explicit operator DTO.User(User user)
        {
            return new DTO.User
            {
                Category = user.Category,
                Id = user.Id,
                UnitId = user.UnitId,
                Name = user.Name,
                PublicKeyEncoded = user.PublicKeyEncoded,
                UnitAdmin = user.UnitAdmin,
            };
        }

        public static explicit operator User(DTO.User user)
        {
            return new User
            {
                Category = user.Category,
                Id = user.Id,
                UnitId = user.UnitId,
                Name = user.Name,
                PublicKeyEncoded = user.PublicKeyEncoded,
                UnitAdmin = user.UnitAdmin,
            };
        }
    }
}