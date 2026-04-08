using System.ComponentModel.DataAnnotations;

namespace EduTrackAcademics.Model

{

	public class CourseBatch
	{

		[Key]

		public string BatchId { get; set; }


		[Required]

		public string CourseId { get; set; }

		public Course Course { get; set; }


		[Required]

		public string InstructorId { get; set; }

		public Instructor Instructor { get; set; }


		public int MaxStudents { get; set; } = 20;

		public int CurrentStudents { get; set; } = 0;


		public bool IsActive { get; set; } = true;
		public DateTime? LastFilledDate { get; set; }

		public ICollection<StudentBatchAssignment> StudentBatchAssignments { get; set; }

	}

}