namespace EduTrackAcademics.Exceptions
{
	public class InstructorNotFoundException : ApplicationException
	{
		public InstructorNotFoundException(string skill)
			: base($"Instructor not found with skill : {skill}")
		{
		}
	}
}	