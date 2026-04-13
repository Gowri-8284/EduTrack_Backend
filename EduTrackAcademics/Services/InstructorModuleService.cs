using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;

namespace EduTrackAcademics.Services
{
	public class InstructorModuleService : IInstructorModuleService
	{
		private readonly IInstructorModuleRepository _repo;

		public InstructorModuleService(IInstructorModuleRepository repo)
		{
			_repo = repo;
		}

		// MODULE

		public async Task<(Module module, string message)> CreateModuleAsync(ModuleDTO dto)
		{
			var newId = await _repo.GenerateModuleIdAsync();

			var module = new Module
			{
				ModuleID = newId,
				CourseId = dto.CourseId,
				Name = dto.Name,
				SequenceOrder = dto.SequenceOrder,
				LearningObjectives = dto.LearningObjectives
			};

			await _repo.AddModuleAsync(module);

			return (module, "Module successfully created");
		}

		public async Task<IEnumerable<object>> GetAllModulesAsync()
		{
			var modules = await _repo.GetAllModulesAsync();

			return modules.Select(m => new
			{
				m.ModuleID,
				m.CourseId,
				m.Name,
				m.SequenceOrder,
				m.LearningObjectives
			});
		}

		public async Task<IEnumerable<object>> GetModulesAsync(string courseId)
		{
			var modules = await _repo.GetModulesByCourseAsync(courseId);

			return modules.Select(m => new
			{
				m.ModuleID,
				m.CourseId,
				m.Name,
				m.SequenceOrder,
				m.LearningObjectives
			});
		}

		public async Task<string> UpdateModuleAsync(string moduleId, ModuleDTO dto)
		{
			var module = await _repo.GetModuleByIdAsync(moduleId);

			if (module == null)
				throw new ModuleNotFoundException(moduleId);

			module.Name = dto.Name;
			module.SequenceOrder = dto.SequenceOrder;
			module.LearningObjectives = dto.LearningObjectives;

			await _repo.UpdateModuleAsync(module);

			return "Module updated successfully";
		}

		public async Task<string> DeleteModuleAsync(string moduleId)
		{
			var deleted = await _repo.DeleteModuleAsync(moduleId);

			if (!deleted)
				throw new ModuleNotFoundException(moduleId);

			return "Module deleted successfully";
		}

		// CONTENT

		public async Task<string> CreateContentAsync(ContentDTO dto)
		{
			if (!await _repo.ModuleExistsAsync(dto.ModuleId))
				throw new ModuleNotFoundException(dto.ModuleId);

			if (!new[] { "Video", "PDF", "Slide", "Lab" }.Contains(dto.ContentType))
				throw new InvalidContentTypeException(dto.ContentType);

			var content = new Content
			{
				ContentID = await _repo.GenerateContentIdAsync(),
				ModuleID = dto.ModuleId,
				Title = dto.Title,
				ContentType = dto.ContentType,
				ContentURI = dto.ContentURI,
				Status = "Draft"
			};

			await _repo.AddContentAsync(content);

			return $"Content created successfully with ID = {content.ContentID}";
		}

		public async Task<List<Content>> GetContentByModuleAsync(string moduleId)
		{
			return await _repo.GetContentByModuleAsync(moduleId);
		}

		public async Task<Content> GetContentAsync(string id)
		{
			var content = await _repo.GetContentByIdAsync(id);
			if (content == null)
				throw new ContentNotFoundException(id);

			return content;
		}

		public async Task<string> UpdateContentAsync(string id, ContentDTO dto)
		{
			var content = await _repo.GetContentByIdAsync(id);
			if (content == null)
				throw new ContentNotFoundException(id);

			content.Title = dto.Title;
			content.ContentType = dto.ContentType;
			content.ContentURI = dto.ContentURI;

			await _repo.UpdateContentAsync(content);

			return "Content updated successfully";
		}

		public async Task<string> PublishContentAsync(string id)
		{
			var content = await _repo.GetContentByIdAsync(id);
			if (content == null)
				throw new ContentNotFoundException(id);

			if (content.Status == "Published")
				throw new ContentAlreadyPublishedException(id);

			content.Status = "Published";

			await _repo.UpdateContentAsync(content);

			return "Content published successfully";
		}

		public async Task<string> DeleteContentAsync(string id)
		{
			var content = await _repo.GetContentByIdAsync(id);
			if (content == null)
				throw new ContentNotFoundException(id);

			await _repo.DeleteContentAsync(content);

			return "Content deleted successfully";
		}

		// dashboard

		public IEnumerable<InstructorCurriculumDTO> GetCurriculumDashboard(string instructorId)
		{
			return _repo.GetCurriculumDashboard(instructorId);
		}
	}
}
