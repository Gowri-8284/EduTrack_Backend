namespace EduTrackAcademics.DTO
{
	public record VerifyEmailDto
	{
		public string Email { get; set; }
		public string Otp { get; set; }
	}
}
