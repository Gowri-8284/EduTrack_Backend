using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using EduTrackAcademics.Services;

namespace EduTrackAcademics.Repository
{
	public class AdminRepository : IAdminRepository
	{
		private readonly EduTrackAcademicsContext _context;
		private readonly IdService _idService;

		public AdminRepository(EduTrackAcademicsContext context, IdService idService)
		{
			_context = context;
			_idService = idService;
		}

		// --- QUALIFICATIONS ---

		public object AddQualification(QualificationDTO dto)
		{
			var qualification = new Qualification
			{
				QualificationId = _idService.GenerateQualificationId(),
				QualificationName = dto.QualificationName,
				Qualificationsh = dto.Qualificationsh,
				QualificationYears = dto.QualificationYears,
				QualificationDescription=dto.QualificationDescription
			};
			_context.Qualification.Add(qualification);
			_context.SaveChanges();
			return new { Message = "Qualification added", Id = qualification.QualificationId };
		}

		public IEnumerable<QualificationDTO> GetAllQualifications()
		{
			return _context.Qualification
				.Select(q => new QualificationDTO { 
				QualificationName = q.QualificationName,
				Qualificationsh = q.Qualificationsh,
				QualificationYears = q.QualificationYears,
				QualificationDescription = q.QualificationDescription

				})
				.ToList();
		}

		public object UpdateQualificationByName(string name, QualificationDTO dto)
		{
			var entity = _context.Qualification.FirstOrDefault(q => q.QualificationName == name);
			if (entity == null) throw new ApplicationException("Qualification not found");

			entity.QualificationName = dto.QualificationName;
			_context.SaveChanges();
			return new { Message = "Qualification updated successfully" };
		}

		public object DeleteQualificationByName(string name)
		{
			var entity = _context.Qualification.FirstOrDefault(q => q.QualificationName == name);
			if (entity == null) throw new ApplicationException("Qualification not found");

			_context.Qualification.Remove(entity);
			_context.SaveChanges();
			return new { Message = "Qualification deleted successfully" };
		}

		// --- PROGRAMS ---

		public object AddProgram(ProgramDTO dto)
		{
			var qualification = _context.Qualification.FirstOrDefault(q => q.QualificationId == dto.QualificationId);
			if (qualification == null) throw new ApplicationException("Qualification does not exist");

			var program = new ProgramEntity
			{
				ProgramId = _idService.GenerateProgramId(),
				ProgramName = dto.ProgramName,
				QualificationId = dto.QualificationId
			};
			_context.Programs.Add(program);
			_context.SaveChanges();
			return new { Message = "Program added", Id = program.ProgramId };
		}

		public IEnumerable<ProgramResponseDTO> GetAllPrograms()
		{
			return _context.Programs
				.Select(p => new ProgramResponseDTO { ProgramName = p.ProgramName, QualificationId = p.QualificationId })
				.ToList();
		}

		public object UpdateProgramByName(string name, ProgramDTO dto)
		{
			var entity = _context.Programs.FirstOrDefault(p => p.ProgramName == name);
			if (entity == null) throw new ApplicationException("Program not found");

			entity.ProgramName = dto.ProgramName;
			entity.QualificationId = dto.QualificationId;
			_context.SaveChanges();
			return new { Message = "Program updated successfully" };
		}

		public object DeleteProgramByName(string name)
		{
			var entity = _context.Programs.FirstOrDefault(p => p.ProgramName == name);
			if (entity == null) throw new ApplicationException("Program not found");

			_context.Programs.Remove(entity);
			_context.SaveChanges();
			return new { Message = "Program deleted successfully" };
		}

		// --- ACADEMIC YEARS ---
		public object AddAcademicYear(AcademicYearDTO dto)
		{
			var program = _context.Programs.FirstOrDefault(p => p.ProgramId == dto.ProgramId);
			if (program == null) throw new ApplicationException("Program does not exist");

			var year = new AcademicYear
			{
				AcademicYearId = _idService.GenerateAcademicYearId(),
				// Now int to int (Assuming DTO is also int)
				YearNumber = dto.YearNumber,
				ProgramId = dto.ProgramId
			};

			_context.AcademicYear.Add(year);
			_context.SaveChanges();

			return new { Message = "Academic year added", Id = year.AcademicYearId };
		}

		public IEnumerable<AcademicYearResponseDTO> GetAllAcademicYears()
		{
			return _context.AcademicYear
				.Select(y => new AcademicYearResponseDTO
				{
					AcademicYearId = y.AcademicYearId,
					YearNumber = y.YearNumber, // int to int
					ProgramId = y.ProgramId
				})
				.ToList();
		}

		public object UpdateAcademicYearByName(string name, AcademicYearDTO dto)
		{
			// Convert the string 'name' from the URL to an int to match the Model
			if (!int.TryParse(name, out int searchYear))
				throw new ApplicationException("Invalid year format. Must be a number.");

			var entity = _context.AcademicYear.FirstOrDefault(y => y.YearNumber == searchYear);

			if (entity == null) throw new ApplicationException("Academic Year not found");

			entity.YearNumber = dto.YearNumber; // int to int
			entity.ProgramId = dto.ProgramId;

			_context.SaveChanges();
			return new { Message = "Academic Year updated successfully" };
		}

		public object DeleteAcademicYearByName(string name)
		{
			// Convert the string 'name' from the URL to an int
			if (!int.TryParse(name, out int searchYear))
				throw new ApplicationException("Invalid year format.");

			var entity = _context.AcademicYear.FirstOrDefault(y => y.YearNumber == searchYear);

			if (entity == null) throw new ApplicationException("Academic Year not found");

			_context.AcademicYear.Remove(entity);
			_context.SaveChanges();
			return new { Message = "Academic Year deleted successfully" };
		}

		// --- RULES ---

		public object AddRule(AcademicRuleDTO dto)
		{
			var existingRule = _context.AcademicRules.FirstOrDefault(r => r.RuleName == dto.RuleName);
			if (existingRule != null) throw new ApplicationException("Rule already exists");

			var rule = new AcademicRule
			{
				RuleId = _idService.GenerateRuleId(),
				RuleName = dto.RuleName,
				RuleValue = dto.RuleValue,
				Description = dto.Description,
				LastUpdated = DateTime.UtcNow
			};
			_context.AcademicRules.Add(rule);
			_context.SaveChanges();
			return new { Message = "Rule added successfully", RuleId = rule.RuleId };
		}

		public IEnumerable<AcademicRuleResponseDTO> GetAllRules()
		{
			return _context.AcademicRules
				.Select(r => new AcademicRuleResponseDTO
				{
					RuleName = r.RuleName,
					RuleValue = r.RuleValue,
					Description = r.Description,
					LastUpdated = r.LastUpdated
				}).ToList();
		}

		public object UpdateRuleByName(string name, AcademicRuleDTO dto)
		{
			var entity = _context.AcademicRules.FirstOrDefault(r => r.RuleName == name);
			if (entity == null) throw new ApplicationException("Rule not found");

			entity.RuleName = dto.RuleName;
			entity.RuleValue = dto.RuleValue;
			entity.Description = dto.Description;
			entity.LastUpdated = DateTime.UtcNow;
			_context.SaveChanges();
			return new { Message = "Rule updated successfully" };
		}

		public object DeleteRuleByName(string name)
		{
			var entity = _context.AcademicRules.FirstOrDefault(r => r.RuleName == name);
			if (entity == null) throw new ApplicationException("Rule not found");

			_context.AcademicRules.Remove(entity);
			_context.SaveChanges();
			return new { Message = "Rule deleted successfully" };
		}
	}
}