using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model

{

	public class Performance
	{

		[Key]

		[Required]

		[RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "ProgressID must be alphanumeric and may include dashes or underscores.")]

		public string ProgressID { get; set; }  // unique code for each course

		[Required]

		[RegularExpression(@"^\d+$", ErrorMessage = "EnrollmentId must be numeric.")]

		public string EnrollmentId { get; set; } // id for student after enrolling, with this we can get details

		[Required]

		[Column(TypeName = "decimal(5,2)")]

		[Range(0, 100, ErrorMessage = "CompletionPercentage must be between 0 and 100.")]

		[RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "CompletionPercentage must be a decimal with up to 3 digits before and 2 digits after the decimal.")]

		public decimal CompletionPercentage { get; set; }


		[Required]

		[DataType(DataType.Date)]

		[RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "LastUpdated must be in yyyy-MM-dd format.")]

		public DateTime LastUpdated { get; set; } //lastmodifieddate 

		[Required]

		[Column(TypeName = "decimal(5,2)")]

		[Range(0, 100, ErrorMessage = "AvgScore must be between 0 and 100.")]

		[RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "AvgScore must be a decimal with up to 3 digits before and 2 digits after the decimal.")]

		public decimal AvgScore { get; set; }


		public string BatchId { get; set; }//fk
		[ForeignKey("BatchId")]

		public CourseBatch courseBatch { get; set; }


		[Required]

		[ForeignKey("Instructor")]

		public string InstructorId { get; set; }


		[Required]

		[ForeignKey("Student")]

		public string StudentId { get; set; }//fk
		public Student Student { get; set; }



	}

}