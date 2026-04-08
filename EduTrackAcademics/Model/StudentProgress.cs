using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class StudentProgress
	{
		[Key]
		[Required]
		public string ProgressID { get; set; }

		[Required]
		[ForeignKey("Student")]
		public string StudentId { get; set; }

		[Required]
		[ForeignKey("Course")]
		public string CourseId { get; set; }

		[Required]
		[ForeignKey("Content")]
		public string ContentId { get; set; }

		public bool IsCompleted { get; set; } = false;

		public DateTime CompletionDate { get; set; } = DateTime.Now;
	}
}
