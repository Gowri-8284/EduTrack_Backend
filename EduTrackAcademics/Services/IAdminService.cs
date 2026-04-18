using EduTrackAcademics.DTO;

using EduTrackAcademics.DTO;

namespace EduTrackAcademics.Services

{

    public interface IAdminService
    {        // 🔹 QUALIFICATIONS
        object AddQualification(QualificationDTO dto);

        IEnumerable<QualificationResponseDTO> GetAllQualification();

        object UpdateQualificationByName(string name, QualificationDTO dto);

        object DeleteQualificationByName(string name);


        // 🔹 PROGRAMS
        object AddProgram(ProgramDTO dto);

        IEnumerable<ProgramResponseDTO> GetAllPrograms();

        object UpdateProgramByName(string name, ProgramDTO dto);

        object DeleteProgramByName(string name);


        // 🔹 ACADEMIC YEARS
        object AddAcademicYear(AcademicYearDTO dto);

        IEnumerable<AcademicYearResponseDTO> GetAllAcademicYears();

        object UpdateAcademicYearByName(string name, AcademicYearDTO dto);

        object DeleteAcademicYearByName(string name);


        // 🔹 RULES
        object AddRule(AcademicRuleDTO dto);

        IEnumerable<AcademicRuleResponseDTO> GetAllRules();

        object UpdateRuleByName(string name, AcademicRuleDTO dto);

        object DeleteRuleByName(string name);

    }

}