namespace PlayerManagementSystem.Models
{
    public class PersonalDetails
    {
        public int Id { get; set; }               // Primary Key for PersonalDetails
        public string ProfilePicUrl { get; set; } // URL for Profile Picture
        public string Name { get; set; }          // Full Name
        public string PhoneNo { get; set; }       // Contact Phone Number
        public string Email { get; set; }         // Email Address
        public DateOnly Dob { get; set; }         // Date of Birth
        public string Gender { get; set; }        // Gender

        // List of Addresses referencing the Address class
        public List<Address> Addresses { get; set; } = new List<Address>();
    }
    
}

