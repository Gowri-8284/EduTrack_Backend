using System.ComponentModel.DataAnnotations;

namespace EduTrackAcademics.DTO
{
	public class StudentDTO
	{
		[Required(ErrorMessage = "Student name is required.")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
		[RegularExpression(@"^[A-Za-z][A-Za-z\.\'\-\s]{1,99}$",
	ErrorMessage = "Name can include letters, spaces, '.', '-' and apostrophes, and must start with a letter.")]
		public string StudentName { get; set; }

		[Required]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string StudentEmail { get; set; }

		public string StudentPhone { get; set; }

		[Required(ErrorMessage = "Qualification is required.")]
		public string StudentQualification { get; set; }

		[Required(ErrorMessage = "Program is required.")]
		public string StudentProgram { get; set; }

		[Required(ErrorMessage = "Academic year is required.")]
		public DateOnly StudentAcademicYear { get; set; }
		public int year { get; set; } = 0;
		[Required(ErrorMessage = "Gender is required.")]
		[RegularExpression(@"^(Male|Female|Non-Binary|Other|Prefer Not To Say)$",
					ErrorMessage = "Gender must be one of: Male, Female, Non-Binary, Other, Prefer Not To Say.")]
		[StringLength(20)]
		public string StudentGender { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,64}$",
	ErrorMessage = "Password must be 8–64 chars and include uppercase, lowercase, number, and special character.")]
		public string StudentPassword { get; set; }
	}
}
