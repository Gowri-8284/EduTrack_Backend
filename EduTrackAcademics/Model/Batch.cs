//using System.ComponentModel.DataAnnotations;

//namespace EduTrackAcademics.Model
//{
//	public class Batch
//	{
//		[Key]
//		[Required]
//		[RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "BatchID must be alphanumeric.")]
//		public string BatchID { get; set; }
//		[Required]
//		public string BatchName { get; set; }
//		[Required]
//		public string CourseId { get; set; }
//		public int Strength { get; set; }
//		//public ICollection<Enrollment> Enrollments { get; set; }
//		public ICollection<Attendance> Attendances { get; set; }
//		public Student Student { get; set; }
//	}
//}
