namespace GesturBee_Backend.DTO
{
    public class ClassInvitationGroupDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public List<InvitationRequestDTO> Requests { get; set; }
    }
}
