using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduTrackAcademics.Model;
using Microsoft.IdentityModel.Tokens;

namespace EduTrackAcademics.AuthFolder
{
	public class JWTTokenGenerator
	{
		private readonly IConfiguration _config;
		public JWTTokenGenerator(IConfiguration configuration)
		{
			_config = configuration;
		}

		public string GenerateToken(Users user, bool rememberMe = false)
		{
			// Ensure secret is at least 32 characters for HmacSha256
			var secret = _config["JwtSettings:Key"] ?? "Default_Secret_Key_For_Development_32_Chars_Long";
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			//7 days for 'Remember Me', otherwise 1 hour
			var expiry = DateTime.UtcNow.AddHours(1);


			var claims = new List<Claim>
		    {
		     new Claim(ClaimTypes.Name, user.Email),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim("role", user.Role ?? "User"),
			new Claim("id", user.UserId.ToString()),
            // NEW: Claim to check if the email was verified through OTP
            new Claim("isVerified", user.IsEmailVerified.ToString().ToLower())
		};

			var token = new JwtSecurityToken(
				_config["JwtSettings:Issuer"] ?? "EduTrackAcademicsAPI",
				_config["JwtSettings:Audience"] ?? "EduTrackAcademicsClient",
				claims,
				expires: expiry,
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}






