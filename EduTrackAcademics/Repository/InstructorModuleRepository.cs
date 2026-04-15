using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Repository
{
	public class InstructorModuleRepository : IInstructorModuleRepository
	{
		private readonly EduTrackAcademicsContext _context;

		public InstructorModuleRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		// MODULE

		public async Task<string> GenerateModuleIdAsync()
		{
			var lastModule = await _context.Modules
				.OrderByDescending(m => m.ModuleID)
				.FirstOrDefaultAsync();

			if (lastModule == null)
				return "M001";

			int num = int.Parse(lastModule.ModuleID.Substring(1));
			return $"M{(num + 1).ToString("D3")}";
		}

		public async Task AddModuleAsync(Module module)
		{
			await _context.Modules.AddAsync(module);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Module>> GetAllModulesAsync()
		{
			return await _context.Modules
				.OrderBy(m => m.SequenceOrder)
				.ToListAsync();
		}

		public async Task<List<Module>> GetModulesByCourseAsync(string courseId)
		{
			return await _context.Modules
				.Where(m => m.CourseId == courseId)
				.OrderBy(m => m.SequenceOrder)
				.ToListAsync();
		}

		public async Task<Module> GetModuleByIdAsync(string moduleId)
		{
			return await _context.Modules
				.FirstOrDefaultAsync(m => m.ModuleID == moduleId);
		}

		public async Task UpdateModuleAsync(Module module)
		{
			_context.Modules.Update(module);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> DeleteModuleAsync(string moduleId)
		{
			var module = await _context.Modules
									   .FirstOrDefaultAsync(m => m.ModuleID == moduleId);

			if (module == null)
				return false;

			_context.Modules.Remove(module);
			await _context.SaveChangesAsync();

			return true;
		}

		// CONTENT

		public async Task<string> GenerateContentIdAsync()
		{
			var last = await _context.Contents
				.OrderByDescending(c => c.ContentID)
				.Select(c => c.ContentID)
				.FirstOrDefaultAsync();

			if (last == null) return "CT001";

			int num = int.Parse(last.Substring(2));
			return $"CT{num + 1:D3}";
		}

		public async Task<bool> ModuleExistsAsync(string moduleId)
		{
			return await _context.Modules.AnyAsync(m => m.ModuleID == moduleId);
		}

		public async Task AddContentAsync(Content content)
		{
			await _context.Contents.AddAsync(content);
			await _context.SaveChangesAsync();
		}

		public async Task<Content> GetContentByIdAsync(string contentId)
		{
			return await _context.Contents.FindAsync(contentId);
		}

		public async Task<List<Content>> GetContentByModuleAsync(string moduleId)
		{
			return await _context.Contents
				.Where(c => c.ModuleID == moduleId)
				.ToListAsync();
		}

		public async Task UpdateContentAsync(Content content)
		{
			_context.Contents.Update(content);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteContentAsync(Content content)
		{
			_context.Contents.Remove(content);
			await _context.SaveChangesAsync();
		}

		// Dashboard

		public IEnumerable<InstructorCurriculumDTO> GetCurriculumDashboard(string instructorId)
		{
			var data = (from b in _context.CourseBatches
						join c in _context.Course on b.CourseId equals c.CourseId
						join bc in _context.BatchConfigs on c.CourseId equals bc.CourseId
						where b.InstructorId == instructorId && b.IsActive == true

						select new InstructorCurriculumDTO
						{
							CourseId = c.CourseId,
							CourseName = c.CourseName,
							Credits = c.Credits,
							DurationInWeeks = c.DurationInWeeks,
							AcademicYearId = c.AcademicYearId,

							BatchId = b.BatchId,
							BatchSize = bc.BatchSize,
							CurrentStudents = b.CurrentStudents,

							// MODULE COUNT
							TotalModules = _context.Modules
								.Count(m => m.CourseId == c.CourseId),

							// ASSESSMENT COUNT
							TotalAssessments = _context.Assessments
								.Count(a => a.CourseId == c.CourseId)
						}).ToList();

			return data;
		}

	}
}
