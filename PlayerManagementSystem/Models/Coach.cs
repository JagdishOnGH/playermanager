namespace PlayerManagementSystem.Models
{
    public class Coach
    {
        public int Id { get; set; }
        public int PersonalDetailsId { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        public int CoachingTeamId { get; set; }
        public Teams CoachingTeam { get; set; }
    }

}

