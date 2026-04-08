namespace EduTrackAcademics.Exceptions
{
	public class ModuleNotFoundException : ApplicationException
	{
		public int StatusCode { get; }

		public ModuleNotFoundException(string moduleId, int statusCode = 404)
			: base($"Module '{moduleId}' not found.")
		{
			StatusCode = statusCode;
		}
	}
}
