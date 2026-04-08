namespace EduTrackAcademics.Exceptions
{
	public class InvalidContentTypeException : ApplicationException
	{
		public int StatusCode { get; }

		public InvalidContentTypeException(string type, int statusCode = 400)
			: base($"Invalid content type: {type}")
		{
			StatusCode = statusCode;
		}
	}
}
