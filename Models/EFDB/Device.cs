using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.EFDB
{
    public class Device
    {
        [Key]
        public Guid Id { get; set; }
         
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        public DateTime LastModified { get; set; }
    }
}