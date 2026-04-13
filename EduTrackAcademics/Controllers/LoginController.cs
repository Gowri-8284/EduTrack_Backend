using EduTrackAcademics.DTO;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Mvc;
using EduTrackAcademics.AuthFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
namespace EduTrackAcademics.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[EnableCors("MyCorsPolicy")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationService _authService;

		public AuthenticationController(IAuthenticationService authService)
		{
			_authService = authService;
		}

		//to verify email and generate otp 

		[AllowAnonymous]
		[HttpPost("generate-OTP")]
		public async Task<IActionResult> GenerateOtp([FromBody] EmailRequest request)
		{
			var otp = await _authService.GenerateOtpAsync(request.Email);
			if (otp == null)
				return NotFound(new { Message = "User not found." });

			return Ok(new { Message = "Verification code sent to your email." });
		}

		//generated otp will be verified here and if valid then email will be verified and user can login

		[AllowAnonymous]
		[HttpPost("verify-Email")]
		public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
		{
			var result = await _authService.VerifyEmailAsync(dto);

			if (!result) return BadRequest(new { Message = "Invalid or expired OTP." });

			return Ok(new { Message = "Email verified successfully." });
		}

		//login with email and password and generate jwt token for authentication and authorization

		[AllowAnonymous]
		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginDTO dto)
		{
			var token = await _authService.LoginAsync(dto);

			if (token == null)
				return Unauthorized(new { Message = "Invalid email or password." });

			return Ok(new { Token = token });
		}

		//forgot password will generate a reset token and send it to the user's email

		[AllowAnonymous]
		[HttpPost("Forgot-Password")]
		public async Task<IActionResult> ForgotPassword([FromBody] EmailRequest request)
		{
			var token = await _authService.GenerateResetTokenAsync(request.Email);

			if (token == null)
				return NotFound(new { Message = "User not found." });

			return Ok(new { Message = "Reset token sent to your email." });
		}

		//reset password will verify the reset token and if valid then update the user's password

		[Authorize]
		[HttpPost("Change-Password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			var authenticatedEmail = User.Identity?.Name; 
			if (string.IsNullOrEmpty(authenticatedEmail))
				return Unauthorized();

			dto.Email = authenticatedEmail;

			var result = await _authService.ChangePasswordAsync(dto);

			if (!result)
				return BadRequest(new { Message = "Invalid token or expired." });

			return Ok(new { Message = "Password has been reset successfully." });
		}

		//logout will invalidate the user's token and remove it from the database

		[Authorize]
		[HttpPost("Logout")]
		public async Task<IActionResult> Logout([FromBody] EmailRequest request)
		{
			var result = await _authService.LogoutAsync(request.Email);

			if (!result) return
					NotFound(new { Message = "User not found." });

			return Ok(new { Message = "Logged out successfully." });
		}
	}
}