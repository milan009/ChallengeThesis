using System.ComponentModel.DataAnnotations;

namespace Models.EFDB
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}