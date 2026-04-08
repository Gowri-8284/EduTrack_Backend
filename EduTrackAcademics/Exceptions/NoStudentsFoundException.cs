namespace EduTrackAcademics.Exceptions
{
	public class NoStudentsFoundException : ApplicationException
	{
		public int StatusCode { get; }
		public NoStudentsFoundException(string message, int statusCode = 404)
			: base(message)
		{
			StatusCode = statusCode;
		}
	}
}
