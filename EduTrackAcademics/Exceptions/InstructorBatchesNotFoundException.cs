namespace EduTrackAcademics.Exceptions
{
	public class InstructorBatchesNotFoundException : ApplicationException
	{
		public int StatusCode { get; }
		public InstructorBatchesNotFoundException(string message, int statusCode = 404)
			: base(message)
		{
			StatusCode = statusCode;
		}
	}
}
