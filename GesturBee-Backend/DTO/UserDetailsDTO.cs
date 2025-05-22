namespace GesturBee_Backend.DTO
{
    public class UserDetailsDTO
    {
        //TODO: Add a constructor for initializing the fields
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
