using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace EduTrackAcademics.AuthFolder
{
	public interface IAuthenticationRepository
	{
		Task<Users> GetUserByEmailAsync(string email);
		Task UpdateUserAsync(Users user);
		Task<Users> GetUserByRefreshTokenAsync(string refreshToken);
	}

	public class AuthenticationRepository : IAuthenticationRepository
	{
		private readonly EduTrackAcademicsContext _context;
		public AuthenticationRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		public async Task<Users> GetUserByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task UpdateUserAsync(Users user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		// In AuthenticationRepository
		public async Task<Users> GetUserByRefreshTokenAsync(string refreshToken)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
		}
	}
}


