using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class StudentAnswer
	{
		[Key]
		[Required]
		public string StudentAnswerId { get; set; }

		[Required]
		[StringLength(300, ErrorMessage = "Answer text cannot exceed 300 characters.")]
		public string Answer { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime createdDate { get; set; }

		[ForeignKey("Question")]
		public string QuestionId { get; set; }

		[ForeignKey("Assessment")]
		public string AssessmentId { get; set; }

		[ForeignKey("Student")]
		public string StudentId { get; set; }
	}
}
