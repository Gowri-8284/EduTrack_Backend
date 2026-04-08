
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class Course
	{
		[Key]
		[Required]
		public string CourseId { get; set; }

		[Required]
		public string CourseName { get; set; }

		[Required]
		public string Program_ID { get; set; }

		//[ForeignKey("Program_ID")]
		//public virtual programs programs { get; set; }

		public string Description { get; set; }

		[Required]
		public int CreditPoints { get; set; }

		[Required]
		public bool CourseStatus { get; set; }

		[Required]
		public int CourseDuration { get; set; } // clearer than DateTime
	}
}
