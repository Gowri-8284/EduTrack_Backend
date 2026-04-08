using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace EduTrackAcademics.Model

{

	public class StudentBatchAssignment
	{
		[Required]
		public string BatchId { get; set; }
		public CourseBatch CourseBatch { get; set; }

		[Required]

		public string StudentId { get; set; }

		public Student Student { get; set; }

		public ICollection<Attendance> Attendances { get; set; }

	}
}
