using EduTrackAcademics.Model;


namespace EduTrackAcademics.Dummy
{
	public class DummyInstructorReg
	{
		public List<Instructor> Instructors{ get; set; } = new List<Instructor>();
		public string GenerateInstructrId()
		{
			int count = Instructors.Count; // how many instructor already registered
			return $"I{(count + 1):D3}"; // e.g., I001, I002, I003 }
		}
	}
}
