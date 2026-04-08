
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{

	public class ProgramEntity
	{
		[Key]
		public string ProgramId { get; set; }

		[Required]
		public string ProgramName { get; set; }

		[Required]
		public string QualificationId { get; set; }

		[ForeignKey(nameof(QualificationId))]
		public Qualification Qualification { get; set; }

		public ICollection<AcademicYear> AcademicYears { get; set; }
	}

}
