using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class Coordinator
	{
		[Key]
		public string CoordinatorId { get; set; }

		[ForeignKey("User")]
		public int? UserId { get; set; }
		public Users User { get; set; } // Navigation property to Users


		[Required(ErrorMessage = "Student name is required.")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
		[RegularExpression(@"^[A-Za-z][A-Za-z\.\'\-\s]{1,99}$",
	ErrorMessage = "Name can include letters, spaces, '.', '-' and apostrophes, and must start with a letter.")]
		public string CoordinatorName { get; set; }

		[Required]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		[StringLength(254)]
		public string CoordinatorEmail { get; set; }

		public string Role { get; set; } = "Coordinator";

		[Required(ErrorMessage = "Phone number is required.")]
		[RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
		[Display(Name = "Mobile (+91)")]
		public long CoordinatorPhone { get; set; }

		[Required(ErrorMessage = "Qualification is required.")]
		public string CoordinatorQualification { get; set; }

		[Required(ErrorMessage = "Experience (years) is required.")]
		[Range(0, 50, ErrorMessage = "Experience must be between 0 and 40 years.")]
		public int CoordinatorExperience { get; set; }

		[Required(ErrorMessage = "Gender is required.")]
		[RegularExpression(@"^(Male|Female|Non-Binary|Other|Prefer Not To Say)$",
			ErrorMessage = "Gender must be one of: Male, Female, Non-Binary, Other, Prefer Not To Say.")]
		public string CoordinatorGender { get; set; }
		public string Resumepath { get; set; }

	}
}
