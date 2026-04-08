using System.ComponentModel.DataAnnotations;

namespace EduTrackAcademics.DTO
{
	public class EmailRequest
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;
	}
}
