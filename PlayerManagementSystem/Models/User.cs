
using System.ComponentModel.DataAnnotations;


namespace PlayerManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid? PalikaId { get; set; }
        public Palika? Palika { get; set; }
         public Guid? WardId { get; set; }  
         public Ward? Ward { get; set; }
        public string Role 
        { 
            get
            {
                if (PalikaId.HasValue)
                    return "PalikaAdmin"; // User is associated with a Palika but no Ward

                if (WardId.HasValue)
                    return "WardAdmin"; // User is associated with a Ward

                throw new Exception(
                    message: "Both WardId and Palika ID is null"); // Default role when no Palika or Ward is assigned
            }
            set{}
        }
    }

    
}