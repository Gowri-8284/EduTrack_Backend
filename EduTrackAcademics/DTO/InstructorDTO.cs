using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EduTrackAcademics.DTO
{
	public class InstructorDTO
	{

		[Required(ErrorMessage = "Student name is required.")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
		[RegularExpression(@"^[A-Za-z][A-Za-z\.\'\-\s]{1,99}$",
	ErrorMessage = "Name can include letters, spaces, '.', '-' and apostrophes, and must start with a letter.")]
		public string InstructorName { get; set; }


		[Required]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		[StringLength(254)]
		public string InstructorEmail { get; set; }


		[Required(ErrorMessage = "Phone number is required.")]
		public long InstructorPhone { get; set; }

		[Required(ErrorMessage = "Qualification is required.")]
		public string InstructorQualification { get; set; }

		[Required(ErrorMessage = "Skills are required.")]
		[StringLength(500)]
		[RegularExpression(@"^\s*[A-Za-z0-9\+\.\#\-\s]{1,30}(\s*,\s*[A-Za-z0-9\+\.\#\-\s]{1,30})*\s*$",
			ErrorMessage = "Provide a comma-separated list of skills (letters/numbers/+, ., #, -).")]
		public string InstructorSkills { get; set; }
		[Range(0, 50, ErrorMessage = "Experience must be between 0 and 40 years.")]
		[Required(ErrorMessage = "Experience (years) is required.")]
		public int InstructorExperience { get; set; }


		public DateOnly InstructorJoinDate { get; set; }

		[Required(ErrorMessage = "Gender is required.")]
		[RegularExpression(@"^(Male|Female|Non-Binary|Other|Prefer Not To Say)$",
			ErrorMessage = "Gender must be one of: Male, Female, Non-Binary, Other, Prefer Not To Say.")]
		public string InstructorGender { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,64}$",
					ErrorMessage = "Password must be 8–64 chars and include uppercase, lowercase, number, and special character.")]
		public string InstructorPassword { get; set; }

		//[Required(ErrorMessage = "Resume is required")]
		public IFormFile InstructorResume { get; set; }
	}
}