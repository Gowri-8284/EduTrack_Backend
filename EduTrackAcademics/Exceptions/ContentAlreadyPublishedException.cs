namespace EduTrackAcademics.Exceptions
{
	public class ContentAlreadyPublishedException : ApplicationException
	{
		public int StatusCode { get; }

		public ContentAlreadyPublishedException(string contentId, int statusCode = 400)
			: base($"Content '{contentId}' is already published.")
		{
			StatusCode = statusCode;
		}
	}
}
