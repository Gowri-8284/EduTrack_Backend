namespace EduTrackAcademics.Exceptions
{
	public class ContentNotFoundException : ApplicationException
	{
		public int StatusCode { get; }

		public ContentNotFoundException(string contentId, int statusCode = 404)
			: base($"Content '{contentId}' not found.")
		{
			StatusCode = statusCode;
		}
	}
}
