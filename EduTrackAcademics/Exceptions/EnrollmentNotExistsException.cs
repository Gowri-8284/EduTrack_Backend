namespace EduTrackAcademics.Exceptions
{
	public class EnrollmentNotExistsException : ApplicationException
	{
		public int StatusCode { get; }
		public EnrollmentNotExistsException(string message, int statusCode = 500) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}
