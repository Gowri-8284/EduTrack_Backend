namespace EduTrackAcademics.Exceptions
{
	public class ProgressRecordAlreadyExistsException : ApplicationException
	{
		public int StatusCode { get; }
		public ProgressRecordAlreadyExistsException(string message, int statusCode = 500) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}
