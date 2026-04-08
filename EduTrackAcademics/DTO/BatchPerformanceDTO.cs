namespace EduTrackAcademics.DTO
{
    public class BatchPerformanceDTO
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public int Marks { get; set; }
        public decimal AvgScore { get; set; }
        public decimal TotalScore { get; set; }

        public decimal CompletionPercentage { get; set; }
        public DateTime? LastUpdated { get; set; }
        

    }
}
