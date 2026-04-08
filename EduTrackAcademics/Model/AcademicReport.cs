using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;//BACKEND
namespace EduTrackAcademics.Model

{


	public class AcademicReport
{

	[Key]


	[RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "ReportId must be alphanumeric and may include dashes or underscores.")]

	public string ReportId { get; set; }


	[Required]

	[StringLength(50)]

	[RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Scope must contain only letters and spaces.")]

	public string Course { get; set; }


	[Required]

	[Column(TypeName = "decimal(5,2)")]

	[RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "CompletionRate must be a decimal with up to 3 digits before and 2 digits after the decimal.")]

	public decimal CompletionRate { get; set; }


	[Required]

	[Column(TypeName = "decimal(5,2)")]

	[RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "AvgScore must be a decimal with up to 3 digits before and 2 digits after the decimal.")]

	public decimal AvgScore { get; set; }


	[Required]

	[Column(TypeName = "decimal(5,2)")]

	[RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "DropOutRate must be a decimal with up to 3 digits before and 2 digits after the decimal.")]

	public decimal DropOutRate { get; set; }


	[Required]

	[DataType(DataType.Date)]

	[RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "GeneratedDate must be in yyyy-MM-dd format.")]

	public DateTime GeneratedDate { get; set; }


	//  Batch Attendance


	[Required]


	[Column(TypeName = "decimal(5,2)")]


	[RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "BatchAttendance must be valid percentage")]


	public decimal BatchAverageAttendance { get; set; }



	//  Student Attendance (average / overall store)


	[Required]


	[Column(TypeName = "decimal(5,2)")]


	[RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "StudentAttendance must be valid percentage")]


	public decimal StudentAttendance { get; set; }






}

}