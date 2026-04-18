namespace EduTrackAcademics.DTO
{
    // Used for adding and updating (POST/PUT)
    public class AcademicYearDTO
    {
        // Changed to int to match the Model and Repository logic
        public int YearNumber { get; set; }
        public string ProgramId { get; set; }
    }

    // Used for fetching the list (GET)
    public class AcademicYearResponseDTO
    {
        public string AcademicYearId { get; set; }
        public int YearNumber { get; set; } // Changed to int
        public string ProgramId { get; set; }

        // Keep as string since ProgramName (e.g., "Computer Science") is text
        public string ProgramName { get; set; }

        public string QualificationName { get; set; } // Optional: Add Qualification if you want to show "Bachelor's"
    }
}