namespace PlayerManagementSystem.Models


{
    public class Manager
    {
        public int Id { get; set; }
        public int PersonalDetailsId { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        public int ManagingTeamId { get; set; }
        public Teams ManagingTeam { get; set; }
    }

    
}