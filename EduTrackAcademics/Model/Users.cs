using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Model
{
	public class Users
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
		public int UserId { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		// Allowed values: "Student", "Instructor", "Coordinator"
		public string Role { get; set; }

		public bool IsEmailVerified { get; set; } = false;
		public string? VerificationOtp { get; set; }
		public DateTime? OtpExpiry { get; set; }

		//for forgot password functionality
		public string? ResetToken { get; set; }
		public DateTime? ResetTokenExpiry { get; set; }

		// --- Refresh Token Logic ---
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiry { get; set; }

		// Navigation properties (optional)
		public Student? Student { get; set; }
		public Instructor? Instructor { get; set; }
		public Coordinator? Coordinator { get; set; }
	}
}

