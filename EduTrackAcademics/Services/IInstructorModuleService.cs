using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Services
{
	public interface IInstructorModuleService
	{
		// MODULE
		Task<(Module module, string message)> CreateModuleAsync(ModuleDTO dto);
		Task<IEnumerable<object>> GetAllModulesAsync();
		Task<IEnumerable<object>> GetModulesAsync(string courseId);
		Task<string> UpdateModuleAsync(string moduleId, ModuleDTO dto);
		Task<string> DeleteModuleAsync(string moduleId);

		// CONTENT
		Task<string> CreateContentAsync(ContentDTO dto);
		Task<List<Content>> GetContentByModuleAsync(string moduleId);
		Task<Content> GetContentAsync(string contentId);
		Task<string> UpdateContentAsync(string id, ContentDTO dto);
		Task<string> PublishContentAsync(string id);
		Task<string> DeleteContentAsync(string id);

		// dashboard
		IEnumerable<InstructorCurriculumDTO> GetCurriculumDashboard(string instrutorId);
	}
}
