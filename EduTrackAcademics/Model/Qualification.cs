
using System.ComponentModel.DataAnnotations;

namespace EduTrackAcademics.Model
{ 
	public class Qualification
	{
		[Key]
		public string QualificationId { get; set; }
		[Required]
		public string QualificationName { get; set; }
		public string Qualificationsh { get; set; }
		public string QualificationYears { get; set; }
		public string QualificationDescription { get; set; }


		public ICollection<ProgramEntity> Programs { get; set; }
	}
}
