namespace EduTrackAcademics.DTO
{
    public class ProgramDTO
    {
        public string ProgramName { get; set; }
        public string QualificationId { get; set; }
    }

    public class ProgramResponseDTO
    {
        public string ProgramId { get; set; } // Must be here!
        public string ProgramName { get; set; }
        public string QualificationId { get; set; }
    }
}