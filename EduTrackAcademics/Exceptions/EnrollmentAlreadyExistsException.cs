namespace EduTrackAcademics.Exceptions
{
	public class EnrollmentAlreadyExistsException : ApplicationException
	{
		public int StatusCode { get; }

		public EnrollmentAlreadyExistsException(string message, int statusCode = 500) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}
