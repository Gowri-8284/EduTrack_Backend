using Microsoft.EntityFrameworkCore;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
namespace EduTrackAcademics.Repository
{
	public class StudentProfileRepository : IStudentProfileRepository
	{
		private readonly EduTrackAcademicsContext _context;
		public StudentProfileRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}
		public async Task<bool> StudentExistsAsync(string studentId)
		{
			return await _context.Student
				.AsNoTracking()
				.AnyAsync(s => s.StudentId == studentId);
		}
		public async Task<List<StudentDTO>> GetAllStudentsAsync()
		{

			var student = await _context.Student.ToListAsync();
			return student.Select(p => new StudentDTO
			{
				StudentName = p.StudentName,
				StudentEmail = p.StudentEmail,
				StudentPhone = p.StudentPhone,
				StudentGender = p.StudentGender
			}).ToList();
		}

		public async Task<StudentDTO> GetPersonalInfoAsync(string studentId)
		{
			return await _context.Student
				.Where(p => p.StudentId == studentId)
				.Select(p => new StudentDTO
				{
					StudentName = p.StudentName,
					StudentEmail = p.StudentEmail,
					StudentPhone = p.StudentPhone,
					StudentGender = p.StudentGender
				})
				.AsNoTracking()
				.FirstOrDefaultAsync();
		}
		public async Task<StudentDTO?> GetProgramDetailsAsync(string studentId)
		{
			return await _context.Student
				.Where(s => s.StudentId == studentId)
				.Select(s => new StudentDTO
				{
					StudentQualification = s.StudentQualification,
					StudentProgram = s.StudentProgram,
					StudentAcademicYear = s.StudentAcademicYear
				})
				.AsNoTracking()
				.FirstOrDefaultAsync();
		}
		public async Task UpdateAdditionalInfoAsync(string studentId, StudentAdditionalDetailsDTO dto)
		{
			var existing = await _context.StudentAdditionalDetails
				.FirstOrDefaultAsync(a => a.StudentId == studentId);
			if (existing == null)
			{
				var newDetail = new StudentAdditionalDetails
				{
					StudentId = studentId,
					Nationality = dto.Nationality,
					Citizenship = dto.Citizenship,
					DayscholarHosteller = dto.dayscholarHosteller,
					Certifications = dto.Certifications,
					Clubs_Chapters = dto.Clubs_Chapters,
					Achievements = dto.Achievements,
					EducationGap = dto.EducationGap
				};
				await _context.StudentAdditionalDetails.AddAsync(newDetail);
			}
			else
			{
				existing.Nationality = dto.Nationality;
				existing.Citizenship = dto.Citizenship;
				existing.DayscholarHosteller = dto.dayscholarHosteller;
				existing.Certifications = dto.Certifications;
				existing.Clubs_Chapters = dto.Clubs_Chapters;
				existing.Achievements = dto.Achievements;
				existing.EducationGap = dto.EducationGap;
			}
			await _context.SaveChangesAsync();
		}

		// Get total credits for completed enrollments
		public async Task<int> GetCreditPointsAsync(string studentId)
		{
			return await _context.Enrollment
				.Where(e => e.StudentId == studentId && e.Status == "Completed")
				.Join(_context.Course, e => e.CourseId, c => c.CourseId, (e, c) => c.Credits)
				.SumAsync();
		}

		// to get assignment due
		public async Task<bool> IsStudentEnrolledInCourseAsync(string studentId, string courseId)
		{
			return await _context.Enrollment.
				AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
		}

		public async Task<(DateTime DueDate, string Type, string CourseName)?> GetAssignmentDetailsAsync(string courseId)
		{
			var result = await _context.Assessments
				.Where(a => a.CourseId == courseId)
				.Join(_context.Course, a => a.CourseId, c => c.CourseId, (a, c)
				=> new {
					a.DueDate,
					a.Type,
					c.CourseName
				})
				.FirstOrDefaultAsync();

			if (result == null)
				return null;
			return (result.DueDate, result.Type, result.CourseName);
		}


	}
}

