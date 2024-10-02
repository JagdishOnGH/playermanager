namespace PlayerManagementSystem.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int PersonalDetailsId { get; set; }
        public PersonalDetails PersonalDetails { get; set; }
        public int PlayingTeamId { get; set; }
        public Teams PlayingTeam { get; set; }
    }
}


