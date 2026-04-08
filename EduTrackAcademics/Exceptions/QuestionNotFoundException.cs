using EduTrackAcademics.DTO;

namespace EduTrackAcademics.Exceptions

{

	public class QuestionNotFoundException : ApplicationException
	{


		public QuestionNotFoundException(string QuestionId)

		: base($"Question '{QuestionId}' not found.")

		{

		}

	}

}