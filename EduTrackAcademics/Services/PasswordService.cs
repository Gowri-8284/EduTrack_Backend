using System.Security.Cryptography;
using System.Text;

namespace EduTrackAcademics.Services
{
	public class PasswordService
	{
		public string GenerateRandomPassword()
		{
			return $"Temp@{RandomNumberGenerator.GetInt32(100000, 999999)}";
		}
	}
}