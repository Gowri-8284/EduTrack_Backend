using System.Text.RegularExpressions;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Services
{
	public static class PasswordHelper
	{
		public static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
	}

	public class RegistrationService : IRegistrationService
	{
		private readonly EduTrackAcademicsContext _context;
		private readonly IRegistrationRepository _repo;
		private readonly IWebHostEnvironment _env;

		public RegistrationService(EduTrackAcademicsContext context, IRegistrationRepository repo, IWebHostEnvironment env)
		{
			_context = context;
			_repo = repo;
			_env = env;
		}
		public async Task RegisterStudentAsync(StudentDTO dto)
		{

			if (!dto.StudentEmail.ToLower().EndsWith("@gmail.com"))
				throw new InvalidEmailDomainException("Only @gmail.com addresses are allowed.");

			var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.StudentEmail);

			if (existingUser != null)
				throw new EmailAlreadyRegisteredException("The Email already registered.");

			var user = new Users
			{
				Email = dto.StudentEmail,
				Password = PasswordHelper.HashPassword(dto.StudentPassword),
				Role = "Student"
			};

			var student = new Student
			{
				StudentName = dto.StudentName,
				StudentEmail = dto.StudentEmail,
				StudentPhone = dto.StudentPhone,
				StudentQualification = dto.StudentQualification,
				StudentProgram = dto.StudentProgram,
				StudentAcademicYear = dto.StudentAcademicYear,
				Year = dto.year,
				StudentGender = dto.StudentGender,
			};

			await _repo.RegisterStudentAsync(student, user);
		}

		public async Task RegisterInstructorAsync(InstructorDTO dto)
		{
			if (!dto.InstructorEmail.ToLower().EndsWith("@gmail.com"))
				throw new InvalidEmailDomainException("Only @gmail.com addresses are allowed.");
			var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.InstructorEmail);
			if (existingUser != null)
				throw new EmailAlreadyRegisteredException("The Email already registered.");

			// --- YOUR ORIGINAL FILE UPLOAD LOGIC UNCHANGED ---
			string folderName = "resumes";
			string pathToSave = Path.Combine(_env.WebRootPath, folderName);
			if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

			string uniqueFileName = $"{Guid.NewGuid()}_{dto.InstructorResume.FileName}";
			string fullPath = Path.Combine(pathToSave, uniqueFileName);

			using (var stream = new FileStream(fullPath, FileMode.Create))
			{
				await dto.InstructorResume.CopyToAsync(stream);
			}
			// ------------------------------------------------

			var user = new Users
			{
				Email = dto.InstructorEmail,
				Password = PasswordHelper.HashPassword(dto.InstructorPassword),
				Role = "Instructor"
			};

			var instructor = new Instructor
			{
				InstructorName = dto.InstructorName,
				InstructorEmail = dto.InstructorEmail,
				InstructorPhone = dto.InstructorPhone,
				InstructorQualification = dto.InstructorQualification,
				InstructorSkills = dto.InstructorSkills,
				InstructorExperience = dto.InstructorExperience,
				InstructorJoinDate = dto.InstructorJoinDate,
				InstructorGender = dto.InstructorGender,
				ResumePath = Path.Combine(folderName, uniqueFileName)
			};

			await _repo.RegisterInstructorAsync(instructor, user);
		}

		public async Task RegisterCoordinatorAsync(CoordinatorDTO dto)
		{
			if (!dto.CoordinatorEmail.ToLower().EndsWith("@gmail.com"))
				throw new InvalidEmailDomainException("Only @gmail.com addresses are allowed.");
			var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.CoordinatorEmail);
			// --- YOUR ORIGINAL FILE UPLOAD LOGIC UNCHANGED ---
			string folderName = "resumes";
			string pathToSave = Path.Combine(_env.WebRootPath, folderName);
			if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

			string uniqueFileName = $"{Guid.NewGuid()}_{dto.Resumepath.FileName}";
			string fullPath = Path.Combine(pathToSave, uniqueFileName);

			using (var stream = new FileStream(fullPath, FileMode.Create))
			{
				await dto.Resumepath.CopyToAsync(stream);
			}
			// ------------------------------------------------

			var user = new Users
			{
				Email = dto.CoordinatorEmail,
				Password = PasswordHelper.HashPassword(dto.CoordinatorPassword),
				Role = "Coordinator"
			};

			var coordinator = new Coordinator
			{
				CoordinatorName = dto.CoordinatorName,
				CoordinatorEmail = dto.CoordinatorEmail,
				CoordinatorPhone = dto.CoordinatorPhone,
				CoordinatorQualification = dto.CoordinatorQualification,
				CoordinatorExperience = dto.CoordinatorExperience,
				CoordinatorGender = dto.CoordinatorGender,
				Resumepath = Path.Combine(folderName, uniqueFileName)
			};

			await _repo.RegisterCoordinatorAsync(coordinator, user);
		}
	}
}