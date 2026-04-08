using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class StudentAdditionalDetails
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[ForeignKey("Student")]
		public string StudentId { get; set; }
		public Student Student { get; set; }
		public string? Nationality { get; set; }
		public string? Citizenship { get; set; }
		public string? DayscholarHosteller { get; set; }
		public string? Certifications { get; set; }
		public string? Clubs_Chapters { get; set; }
		public string? Achievements { get; set; }
		public int? EducationGap { get; set; }
	}
}