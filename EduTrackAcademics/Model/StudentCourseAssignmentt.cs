using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class StudentCourseAssignment
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string StudentId { get; set; }

		[ForeignKey(nameof(StudentId))]
		public Student Student { get; set; }

		[Required]
		public string CourseId { get; set; }

		[ForeignKey(nameof(CourseId))]
		public Course Course { get; set; }
	}
}
