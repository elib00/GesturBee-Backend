namespace GesturBee_Backend.DTO
{
    public class UserProfileDTO
    {
        public int UserProfileId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? ContactNumber { get; set; }

        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
