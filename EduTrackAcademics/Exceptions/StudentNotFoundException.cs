namespace EduTrackAcademics.Exceptions
{
	public class StudentNotFoundException : ApplicationException
	{
		public int StatusCode { get; }
		public StudentNotFoundException(string message, int statusCode = 404)
   : base(message)
		{
			StatusCode = statusCode;
		}
	}
}
