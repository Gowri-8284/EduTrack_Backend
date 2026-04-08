using EduTrackAcademics.DTO;
using EduTrackAcademics.Repository;
using Humanizer;

namespace EduTrackAcademics.Services

{

	public class AdminService : IAdminService
	{
		private readonly IAdminRepository _repo;


		public AdminService(IAdminRepository repo)

		{
			_repo = repo;

		}

        // --- QUALIFICATIONS ---
		public object AddQualification(QualificationDTO dto)

        {            return _repo.AddQualification(dto);

        }

        public IEnumerable<QualificationResponseDTO> GetAllQualification()

		{
			return _repo.GetAllQualifications();

		}

		public object UpdateQualificationByName(string name, QualificationDTO dto)

		{
			return _repo.UpdateQualificationByName(name, dto);

		}

		public object DeleteQualificationByName(string name)

		{
			return _repo.DeleteQualificationByName(name);

		}


        // --- PROGRAMS ---
		public object AddProgram(ProgramDTO dto)

        {            return _repo.AddProgram(dto);

        }

public IEnumerable<ProgramResponseDTO> GetAllPrograms()

{
	return _repo.GetAllPrograms();

}

public object UpdateProgramByName(string name, ProgramDTO dto)

{
	return _repo.UpdateProgramByName(name, dto);

}

public object DeleteProgramByName(string name)

{
	return _repo.DeleteProgramByName(name);

}


// --- ACADEMIC YEARS ---
public object AddAcademicYear(AcademicYearDTO dto)

{
	return _repo.AddAcademicYear(dto);

}

public IEnumerable<AcademicYearResponseDTO> GetAllAcademicYears()

{
	return _repo.GetAllAcademicYears();

}

public object UpdateAcademicYearByName(string name, AcademicYearDTO dto)

{
	return _repo.UpdateAcademicYearByName(name, dto);

}

public object DeleteAcademicYearByName(string name)

{

	return _repo.DeleteAcademicYearByName(name);

}



// --- RULES ---
public object AddRule(AcademicRuleDTO dto)

{

	return _repo.AddRule(dto);

}


public IEnumerable<AcademicRuleResponseDTO> GetAllRules()

{

	return _repo.GetAllRules();

}


public object UpdateRuleByName(string name, AcademicRuleDTO dto)

{

	return _repo.UpdateRuleByName(name, dto);

}


public object DeleteRuleByName(string name)

{

	return _repo.DeleteRuleByName(name);

}

    }

}