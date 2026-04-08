using EduTrackAcademics.Data;

namespace EduTrackAcademics.Services
{
	public class IdService
	{
		private readonly EduTrackAcademicsContext _context;

		public IdService(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		// STUDENT ID → S001
		public string GenerateStudentId()
		{
			int count = _context.Student.Count();
			return $"S{(count + 1):D3}";
		}
		public string GenerateRuleId()
		{
			int count = _context.Student.Count();
			return $"R{(count + 1):D3}";
		}

		// INSTRUCTOR ID → I001
		public string GenerateInstructorId()
		{
			int count = _context.Instructor.Count();
			return $"I{(count + 1):D3}";
		}
		public string GenerateQualificationId()
		{
			int count = _context.Qualification.Count();
			return $"Q{(count + 1):D3}";
		}

		// PROGRAM → P001
		public string GenerateProgramId()
		{
			int count = _context.Programs.Count();
			return $"P{(count + 1):D3}";
		}

		// ACADEMIC YEAR → AY001
		public string GenerateAcademicYearId()
		{
			int count = _context.AcademicYear.Count();
			return $"AY{(count + 1):D3}";
		}
	}
}