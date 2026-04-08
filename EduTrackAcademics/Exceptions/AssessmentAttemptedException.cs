namespace EduTrackAcademics.Exceptions
{
	public class AssessmentAttemptedException : ApplicationException
	{
		public int StatusCode { get; }
		public AssessmentAttemptedException(string message, int statusCode = 500) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}
