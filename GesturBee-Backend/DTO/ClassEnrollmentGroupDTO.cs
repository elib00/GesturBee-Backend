namespace GesturBee_Backend.DTO
{
    public class ClassEnrollmentGroupDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public List<EnrollmentRequestDTO> Requests { get; set; }
    }
}
