
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;


	namespace EduTrackAcademics.Model
	{

	public class AcademicYear
	{
		[Key]
		public string AcademicYearId { get; set; }

		[Required]
		public int YearNumber { get; set; }

		[Required]
		public string ProgramId { get; set; }

		[ForeignKey(nameof(ProgramId))]
		public ProgramEntity Program { get; set; }

		public ICollection<Course> Courses { get; set; }
	}
}

