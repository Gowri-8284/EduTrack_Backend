using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EduTrackAcademics.Model
{
	public class Content
	{
		[Key]
		[Required]
		[RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "ContentId must be AlphaNumeric.")]
		public string ContentID { get; set; }

		[Required]
		[ForeignKey("Module")]
		public string ModuleID { get; set; }
		public Module Module { get; set; }

		[Required]
		[RegularExpression(@"^(Video|PDF|Slide|Lab)$", ErrorMessage = "Content must be Video, PDF, Slide, or Lab")]
		public string ContentType { get; set; }
		[Required]
		[StringLength(150)]
		public string Title { get; set; }
		[Required]
		[Url(ErrorMessage = "Invalid content URL.")]
		public string ContentURI { get; set; }
		public TimeSpan? Duration { get; set; }
		[Required]
		[RegularExpression(@"^(Draft|Published)$", ErrorMessage = "Status must be Draft or Published.")]
		public string Status { get; set; } = "Draft";
	}
}
