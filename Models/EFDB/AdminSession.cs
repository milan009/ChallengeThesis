using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.EFDB
{
    public class AdminSession
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Expires { get; set; }

        [ForeignKey("Admin")]
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}
