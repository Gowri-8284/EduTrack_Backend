using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class Instructor
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string InstructorId { get; set; }

		[ForeignKey("User")]
		public int? UserId { get; set; }
		public Users User { get; set; } // Navigation property to Users

		[Required(ErrorMessage = "Student name is required.")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
		[RegularExpression(@"^[A-Za-z][A-Za-z\.\'\-\s]{1,99}$",
	ErrorMessage = "Name can include letters, spaces, '.', '-' and apostrophes, and must start with a letter.")]
		public string InstructorName { get; set; }

		[Required]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		[StringLength(254)]
		public string InstructorEmail { get; set; }

		public string Role { get; set; } = "Instructor";

		[Required(ErrorMessage = "Phone number is required.")]
		[RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
		[Display(Name = "Mobile (+91)")]
		public long InstructorPhone { get; set; }


		[Required(ErrorMessage = "Qualification is required.")]
		public string InstructorQualification { get; set; }

		[Required(ErrorMessage = "Skills are required.")]
		[StringLength(500)]
		[RegularExpression(@"^\s*[A-Za-z0-9\+\.\#\-\s]{1,30}(\s*,\s*[A-Za-z0-9\+\.\#\-\s]{1,30})*\s*$",
			ErrorMessage = "Provide a comma-separated list of skills (letters/numbers/+, ., #, -).")]
		public string InstructorSkills { get; set; }

		[Required(ErrorMessage = "Experience (years) is required.")]
		[Range(0, 50, ErrorMessage = "Experience must be between 0 and 40 years.")]
		public int InstructorExperience { get; set; }

		[Required(ErrorMessage = "Join date is required.")]
		[DataType(DataType.Date)]
		public DateOnly InstructorJoinDate { get; set; }

		[Required(ErrorMessage = "Gender is required.")]
		[RegularExpression(@"^(Male|Female|Non-Binary|Other|Prefer Not To Say)$",
			ErrorMessage = "Gender must be one of: Male, Female, Non-Binary, Other, Prefer Not To Say.")]
		public string InstructorGender { get; set; }

		public string ResumePath { get; set; }
	}
}


