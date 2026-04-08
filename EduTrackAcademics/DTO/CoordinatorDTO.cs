using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


public class CoordinatorDTO
{
	[Required(ErrorMessage = "Coordinator name is required.")]
	[StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
	public string CoordinatorName { get; set; }

	[Required]
	[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
	[StringLength(254)]
	public string CoordinatorEmail { get; set; }

	[Required(ErrorMessage = "Phone number is required.")]
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

	[Required(ErrorMessage = "Password is required.")]
	[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,64}$",
					ErrorMessage = "Password must be 8–64 chars and include uppercase, lowercase, number, and special character.")]
	public string CoordinatorPassword { get; set; }
	public IFormFile Resumepath { get; set; }
}