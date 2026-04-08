using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class Submission
	{
		[Key]
		[Required]
		public string SubmissionId { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime SubmissionDate { get; set; }
		public int Score { get; set; }

		[Required]
		[StringLength(500)]
		public string Feedback { get; set; }

		[ForeignKey("Assessment")]
		public string AssessmentId { get; set; }

		[ForeignKey("Student")]
		public string StudentID { get; set; }
	}
}
