using System.ComponentModel.DataAnnotations;

namespace EduTrackAcademics.Model
{
	public class AcademicRule
	{
		[Key]
		public string RuleId { get; set; }
		public string RuleName { get; set; }   // e.g. "MinAssignmentScore"
		public string RuleValue { get; set; }  // e.g. "40"
		public string Description { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
