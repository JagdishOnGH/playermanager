
using System.ComponentModel.DataAnnotations;


namespace PlayerManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Userrole Role { get; set; }
    }

    public enum Userrole
    {
        ward,
        palika,
    }
}