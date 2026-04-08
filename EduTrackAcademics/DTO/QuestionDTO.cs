namespace EduTrackAcademics.DTO
{
	public class QuestionDTO
	{
		public string AssessmentId { get; set; }
		public string QuestionType { get; set; }
		public string QuestionText { get; set; }
		public string? OptionA { get; set; }
		public string? OptionB { get; set; }
		public string? OptionC { get; set; }
		public string? OptionD { get; set; }
		public string? CorrectOption { get; set; }
		public int Marks { get; set; }
	}
}
