using EduTrackAcademics.Model;

namespace EduTrackAcademics.Dummy
{
	public  class DummyStudent
	{
		public List<Student> Students { get; set; } = new List<Student>();
		public  string GenerateStudentId()
		{
			int count = Students.Count; // how many students already registered
			return $"S{(count + 1):D3}"; // e.g., S001, S002, S003 }
		}
	}
}
	