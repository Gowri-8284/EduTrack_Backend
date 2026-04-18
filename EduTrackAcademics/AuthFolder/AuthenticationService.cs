using System.Security.Cryptography;
using EduTrackAcademics.AuthFolder;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using EduTrackAcademics.Services;
using Humanizer;
using NuGet.Common;

namespace EduTrackAcademics.AuthFolder
{
	public interface IAuthenticationService
	{
		Task<LoginResponseDTO> LoginAsync(LoginDTO dto);
		Task<string> GenerateResetTokenAsync(string email);
		Task<bool> ChangePasswordAsync(ResetPasswordDto dto);
		Task<string> GenerateOtpAsync(string email);
		Task<bool> VerifyEmailAsync(VerifyEmailDto dto);
		Task<bool> LogoutAsync(string email);
	}

	public class AuthenticationService : IAuthenticationService
	{
		private readonly IAuthenticationRepository _repo;
		private readonly IEmailService _emailService;
		private readonly JWTTokenGenerator _jwtService;

		public AuthenticationService(IAuthenticationRepository repo, IEmailService emailService, JWTTokenGenerator jwtService)
		{
			_repo = repo;
			_emailService = emailService;
			_jwtService = jwtService;
		}

		// 1. LOGIN
		public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto)
		{
			var user = await _repo.GetUserByEmailAsync(dto.Email);
			if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
				return null;

			if (!user.IsEmailVerified)
			{
				return new LoginResponseDTO { Message = "Email not verified" };
			}

			var accessToken = _jwtService.GenerateToken(user);
			var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Long lived
			await _repo.UpdateUserAsync(user);

			return new LoginResponseDTO { AccessToken = accessToken, RefreshToken = refreshToken };
		}
		// 2. GENERATE OTP (Email Verification)
		public async Task<string> GenerateOtpAsync(string email)
		{
			var user = await _repo.GetUserByEmailAsync(email);
			if (user == null) return null;

			var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
			user.VerificationOtp = otp;
			user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

			await _repo.UpdateUserAsync(user);
			await _emailService.SendEmailAsync(
	        email,
	        "EduTrack – Email Verification Code",
             $@"
                   <p>Dear User,</p>
                   <p>Thank you for registering with <strong>EduTrack</strong>.</p>
                   <p>To complete your registration, please use the One-Time Password (OTP) provided below:</p>
                   <h2 style='color:#2F80ED;'>{otp}</h2>
                   <p>This OTP is valid for a limited time. Please do not share it with anyone for security reasons.</p>
                   <p>If you did not initiate this registration, please ignore this email.</p>
				   <br/>
				   <p>Best regards,</p>
				   <p><strong>EduTrack Team</strong></p>
				   <p><small>This is an automated email. Please do not reply.</small></p>
              ");
			return otp;
		}

		// 3. VERIFY EMAIL
		public async Task<bool> VerifyEmailAsync(VerifyEmailDto dto)
		{
			var user = await _repo.GetUserByEmailAsync(dto.Email);

			if (user == null || user.VerificationOtp != dto.Otp || user.OtpExpiry < DateTime.UtcNow)
				return false;

			user.IsEmailVerified = true;
			user.VerificationOtp = null; // Clear OTP after use
			user.OtpExpiry = null;

			await _repo.UpdateUserAsync(user);
			return true;
		}
		public async Task<LoginResponseDTO> RefreshTokenAsync(string token)
		{
			var user = await _repo.GetUserByRefreshTokenAsync(token);

			if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
				return null;

			// Generate new pair (Token Rotation)
			var newAccessToken = _jwtService.GenerateToken(user);
			var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
			await _repo.UpdateUserAsync(user);

			return new LoginResponseDTO { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
		}

		// 4. GENERATE RESET TOKEN (Forgot Password)
		public async Task<string> GenerateResetTokenAsync(string email)
		{
			var user = await _repo.GetUserByEmailAsync(email);
			if (user == null) return null;

			// 1. Generate unique token
			var token = Guid.NewGuid().ToString();
			user.ResetToken = token;
			user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
			await _repo.UpdateUserAsync(user);

			// 2. Create the link for your React frontend (localhost:3000)// We pass both token and email as URL parameters
			var resetLink = $"https://localhost:5173/reset-password?token={token}&email={email}";

			// 3. Create a professional HTML body
			var emailBody = $@"<div style='font-family: sans-serif; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;'>   
			<h2 style='color: #7c3aed;'>EduTrack Password Reset</h2>   
			<p>We received a request to reset your password. Click the button below to proceed:</p>    
			<a href='{resetLink}'        
			style='display: inline-block; background-color: #7c3aed; color: white; padding: 12px 24px;              
			text-decoration: none; border-radius: 6px; font-weight: bold; margin: 15px 0;'>      
			Reset Password    </a>    
			<p style='font-size: 0.875rem; color: #64748b;'>        
			This link expires in 30 minutes. If you didn't request this, you can safely ignore this email.   
			</p>
			</div>";

			// 4. Send using your existing EmailService
			await _emailService.SendEmailAsync(email, "Reset Your EduTrack Password", emailBody);

			return token;
		}


		// 5. RESET PASSWORD
		// 5. Change Password (Internal Profile Update)
		public async Task<bool> ChangePasswordAsync(ResetPasswordDto dto)
		{
		var user = await _repo.GetUserByEmailAsync(dto.Email);
		if (user == null)
		return false;

		user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

		// Clear any existing reset tokens (Cleanup)
		user.ResetToken = null;
		user.ResetTokenExpiry = null;
			await _repo.UpdateUserAsync(user);

        return true;

        }

		// 6. LOGOUT
		// revoke the refresh token
		public async Task<bool> LogoutAsync(string email)
		{
			var user = await _repo.GetUserByEmailAsync(email);
			if (user == null) return false;

			user.RefreshToken = null;
			user.RefreshTokenExpiry = null;
			await _repo.UpdateUserAsync(user);
			return true;
		}
	}
}





