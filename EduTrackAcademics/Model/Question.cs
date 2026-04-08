using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class Question
	{
		[Key]
		[Required]
		[RegularExpression(@"^Q[0-9]{3,}$", ErrorMessage = "QuestionID must be like Q001.")]
		public string QuestionId { get; set; }

		[Required]
		[ForeignKey("Assessment")]
		public string AssessmentId { get; set; }
		public Assessment Assessment { get; set; }

		[Required]
		[RegularExpression(@"^(MCQS|TRUE/FALSE|DESCRIPTION)$", ErrorMessage = "Type must be Mcq's, True/False, Description.")]
		public string QuestionType { get; set; }

		[Required]
		[StringLength(500, ErrorMessage = "Question text cannot exceed 500 characters.")]
		public string QuestionText { get; set; }

		public string? OptionA { get; set; }
		public string? OptionB { get; set; }
		public string? OptionC { get; set; }
		public string? OptionD { get; set; }
		[RegularExpression(@"^(A|B|C|D|True|False)?$", ErrorMessage = "Correct option must be A/B/C/D or True/False.")]
		public string? CorrectOption { get; set; }

		//[Required]
		//[StringLength(500, ErrorMessage = "Answer text cannot exceed 300 characters.")]
		//public string? AnswerText { get; set; }

		[Required]
		[Range(1, 100)]
		public int Marks { get; set; }

		public int OrderNo { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime createdDate { get; set; } = DateTime.Now;
	}
}
