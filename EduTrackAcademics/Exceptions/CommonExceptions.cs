using System;

namespace EduTrackAcademics.Exceptions


{


	public class InvalidInputException : Exception

	{

		public InvalidInputException(string message) : base(message) { }


	}

	public class DataNotFoundException : Exception

	{

		public DataNotFoundException(string message) : base(message) { }


	}


}