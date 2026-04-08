using EduTrackAcademics.Model;

namespace EduTrackAcademics.Repository

{

	public interface IInstructorModuleRepository
	{

		// Module
		Task<string> GenerateModuleIdAsync();

		Task AddModuleAsync(Module module);

		Task<List<Module>> GetModulesByCourseAsync(string courseId);

		Task<Module> GetModuleByIdAsync(string moduleId);

		Task UpdateModuleAsync(Module module);

		Task<bool> DeleteModuleAsync(string moduleId);


		// Content
		Task<string> GenerateContentIdAsync();

		Task<bool> ModuleExistsAsync(string moduleId);

		Task AddContentAsync(Content content);

		Task<Content> GetContentByIdAsync(string contentId);

		Task<List<Content>> GetContentByModuleAsync(string moduleId);

		Task UpdateContentAsync(Content content);

		Task DeleteContentAsync(Content content);

	}

}