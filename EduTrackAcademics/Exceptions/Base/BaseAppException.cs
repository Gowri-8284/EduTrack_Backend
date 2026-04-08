using System;

namespace EduTrackAcademics.Exceptions
{
	public class BaseAppException : Exception
	{
		public int StatusCode { get; }

		public BaseAppException(string message, int statusCode)
			: base(message)
		{
			StatusCode = statusCode;
		}
	}
}