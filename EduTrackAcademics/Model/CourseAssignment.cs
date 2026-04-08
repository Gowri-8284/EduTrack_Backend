using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class CourseAssignment
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string CourseId { get; set; }

		[ForeignKey(nameof(CourseId))]
		public Course Course { get; set; }

		[Required]
		public string InstructorId { get; set; }

		[ForeignKey(nameof(InstructorId))]
		public Instructor Instructor { get; set; }

		public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
	}
}
