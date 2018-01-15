using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.EFDB
{
    public class Unit
    {
        [Key]
        [DatabaseG‌​enerated(DatabaseGen‌​eratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime LastModified { get; set; }
    }
}