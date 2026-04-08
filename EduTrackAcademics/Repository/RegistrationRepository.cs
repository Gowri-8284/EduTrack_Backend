using EduTrackAcademics.Data;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Repository
{
	public class RegistrationRepository : IRegistrationRepository
	{
		private readonly EduTrackAcademicsContext _context;

		public RegistrationRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		public async Task RegisterStudentAsync(Student student, Users user)
		{
			//await ValidateEmail(user.Email);

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				_context.Users.Add(user);
				await _context.SaveChangesAsync();

				student.UserId = user.UserId;
				student.StudentId = $"S{(_context.Student.Count() + 1):D3}";

				_context.Student.Add(student);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task RegisterInstructorAsync(Instructor instructor, Users user)
		{
			//await ValidateEmail(user.Email);

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				_context.Users.Add(user);
				await _context.SaveChangesAsync();

				instructor.UserId = user.UserId;
				instructor.InstructorId = $"I{(_context.Instructor.Count() + 1):D3}";

				_context.Instructor.Add(instructor);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task RegisterCoordinatorAsync(Coordinator coordinator, Users user)
		{
			//await ValidateEmail(user.Email);

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				_context.Users.Add(user);
				await _context.SaveChangesAsync();

				coordinator.UserId = user.UserId;
				coordinator.CoordinatorId = $"C{(_context.Coordinator.Count() + 1):D3}";

				_context.Coordinator.Add(coordinator);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}
	}
}