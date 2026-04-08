namespace EduTrackAcademics.Exceptions

{
	public class BatchNotFoundException : ApplicationException
	{
		public int StatusCode { get; }
		public BatchNotFoundException(string message, int statusCode = 404)
			: base(message)
		{
			StatusCode = statusCode;
		}
	}
}

