using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model

{
	public class Attendance
	{

		[Key]

		[Required]

		[RegularExpression(@"AT[0-9]{3,}$", ErrorMessage = "AttendanceID must be like AT001")]

		public string AttendanceID { get; set; }


		[Required]

		[ForeignKey("Enrollment")]

		public string EnrollmentID { get; set; }

		public Enrollment Enrollment { get; set; }

	
		//[ForeignKey(nameof(StudentBatchAssignmentId))]
		//[ForeignKey(nameof(StudentBatchAssignmentId))]

		[Required]
		public string StudentId { get; set; }

		[ForeignKey("StudentBatchAssignment")]
		[Required]
		public string BatchId { get; set; }
		public StudentBatchAssignment StudentBatchAssignment { get; set; }


		[Required]

		[DataType(DataType.Date)]

		public DateTime SessionDate { get; set; }


		[Required]

		[RegularExpression(@"^(Online|Classroom)$", ErrorMessage = "Mode must be Online or Classroom.")]

		public string Mode { get; set; }


		[Required]

		[RegularExpression(@"^(Present|Absent)$", ErrorMessage = "Status must be Present or Absent.")]

		public string Status { get; set; }


		public string? UpdateReason { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public bool IsDeleted { get; set; } = false;

		public string? DeletionReason { get; set; }
		public DateTime? DeletionDate { get; set; }
}
}
