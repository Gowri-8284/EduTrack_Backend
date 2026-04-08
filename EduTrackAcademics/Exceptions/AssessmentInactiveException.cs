namespace EduTrackAcademics.Exceptions
{
	public class AssessmentInactiveException : ApplicationException
	{
		public int StatusCode { get; }

		public AssessmentInactiveException(string message, int statusCode = 500) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}
