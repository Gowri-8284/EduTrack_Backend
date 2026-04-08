

namespace EduTrackAcademics.Exceptions
{
	public class CourseNotFoundException : ApplicationException
	{
		public CourseNotFoundException(string id)
			: base($"Course not found for id : {id}")
		{
		}
	}
}