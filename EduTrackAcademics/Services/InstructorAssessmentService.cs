using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Humanizer;
using NuGet.Protocol.Core.Types;

namespace EduTrackAcademics.Services

{

	public class InstructorAssessmentService : IInstructorAssessmentService
	{

		private readonly IInstructorAssessmentRepository _repo;


		public InstructorAssessmentService(IInstructorAssessmentRepository repo)

		{

			_repo = repo;

		}



    // ASSESSMENT
	public async Task<string> CreateAssessmentAsync(AssessmentDTO dto)

    {

      var id = await _repo.GenerateAssessmentIdAsync();


		var dueDateUtc = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc);


		var assessment = new Assessment
		{

			AssessmentID = id,

			CourseId = dto.CourseId,

			Type = dto.Type,

			MaxMarks = dto.MaxMarks,

			DueDate = dueDateUtc,

			Status = dueDateUtc < DateTime.UtcNow ? "Closed" : "Open"
		};


		await _repo.AddAssessmentAsync(assessment);


      return $"Assessment created with ID {id} and status {assessment.Status}";

    }


    public async Task<List<Assessment>> GetAllAssessmentsAsync()

		{

			return await _repo.GetAllAssessmentsAsync();

		}


		public async Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date)

		{

			return await _repo.GetAssessmentsByDateAsync(date);

		}


		public async Task<Assessment> GetAssessmentByIdAsync(string id)

		{

			var assessment = await _repo.GetAssessmentByIdAsync(id);

			if (assessment == null)

				throw new ApplicationException("Assessment not found");

			return assessment;

		}


		public async Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId)

		=> await _repo.GetAssessmentsByCourseAsync(courseId);


		public async Task<string> UpdateAssessmentAsync(string id, AssessmentDTO dto)

		{

			var assessment = await _repo.GetAssessmentByIdAsync(id);

			if (assessment == null)

				throw new ApplicationException("Assessment not found");


			assessment.Type = dto.Type;

			assessment.MaxMarks = dto.MaxMarks;

			assessment.DueDate = dto.DueDate;


			await _repo.UpdateAssessmentAsync(assessment);

			return "Assessment updated successfully";

		}


		public async Task<string> DeleteAssessmentAsync(string id)

		{

			var assessment = await _repo.GetAssessmentByIdAsync(id);

			if (assessment == null)

				throw new ApplicationException("Assessment not found");


			await _repo.DeleteAssessmentAsync(assessment);

			return "Assessment deleted successfully";

		}




    //public async Task<SubmissionResultDTO> AddFeedbackAsync(UpdateSubmissionDto dto)//{//  var submission = await _repo.GetSubmissionAsync(dto.StudentId, dto.AssessmentId);//  if (submission == null)//  {//    return new SubmissionResultDTO//    {//      IsSubmitted = false,//      Score = 0,//      Percentage = 0//    };//  }//  submission.Feedback = dto.Feedback;//  submission.Score = dto.Score;//  await _repo.UpdateSubmissionAsync(submission);//  var totalMarks = await _repo.GetTotalMarksAsync(dto.AssessmentId);//  double percentage = 0;//  if (totalMarks > 0)//  {//    percentage = ((double)dto.Score / totalMarks) * 100;//  }//  return new SubmissionResultDTO//  {//    IsSubmitted = true,//    Score = dto.Score,//    Percentage = percentage//  };//}// QUESTIONS
	public async Task<string> AddQuestionAsync(QuestionDTO dto)

    {

      var id = await _repo.GenerateQuestionIdAsync();


		var question = new Question
		{

			QuestionId = id,

			AssessmentId = dto.AssessmentId,

			QuestionType = dto.QuestionType,

			QuestionText = dto.QuestionText,

			OptionA = dto.OptionA,

			OptionB = dto.OptionB,

			OptionC = dto.OptionC,

			OptionD = dto.OptionD,

			CorrectOption = dto.CorrectOption,

			Marks = dto.Marks

		};


		await _repo.AddQuestionAsync(question);

      return $"Question added with ID {id}";

    }



public async Task<Question> GetQuestionByIdAsync(string id)

{

	var question = await _repo.GetQuestionByIdAsync(id);

	if (question == null)

		throw new QuestionNotFoundException(id);

	return question;

}


public async Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId)

=> await _repo.GetQuestionsByAssessmentAsync(assessmentId);


public async Task<string> UpdateQuestionAsync(string id, QuestionDTO dto)

{

	var question = await _repo.GetQuestionByIdAsync(id);

	if (question == null)

		throw new QuestionNotFoundException(id);


	question.QuestionText = dto.QuestionText;

	question.OptionA = dto.OptionA;

	question.OptionB = dto.OptionB;

	question.OptionC = dto.OptionC;

	question.OptionD = dto.OptionD;

	question.CorrectOption = dto.CorrectOption;

	question.Marks = dto.Marks;


	await _repo.UpdateQuestionAsync(question);

	return "Question updated successfully";

}


public async Task<string> DeleteQuestionAsync(string QuestionId)

{

	var question = await _repo.GetQuestionByIdAsync(QuestionId);

	if (question == null)

		throw new QuestionNotFoundException(QuestionId);


	await _repo.DeleteQuestionAsync(question);

	return "Question deleted successfully";

}


// fetching data// ✅ GET STATUS
public async Task<string> GetStatus(string studentId, string assessmentId)

{

	var submission = await _repo.GetSubmission(studentId, assessmentId);


	if (submission == null)

		return "Not Started";


	if (submission.Score > 0)

		return "Completed";


	return "InProgress";

}


// ✅ GET RESULT
public async Task<SubmissionResultDTO> GetResult(string studentId, string assessmentId)

{

	var submission = await _repo.GetSubmission(studentId, assessmentId);


	if (submission == null)

		return null;


	var totalMarks = await _repo.GetTotalMarks(assessmentId);


	double percentage = totalMarks == 0

	? 0

	: (submission.Score * 100.0) / totalMarks;


	return new SubmissionResultDTO
	{

		IsSubmitted = submission.Score > 0,   // ✅ derived        
		Score = submission.Score,

		Percentage = percentage               // ✅ calculated      
	};

	}


	// 🔹 Get single submission (for resume UI)
	public async Task<Submission> GetSubmission(string studentId, string assessmentId)

	{

		return await _repo.GetSubmission(studentId, assessmentId);

	}


	// 🔹 Get all submissions (dashboard)
	public async Task<List<Submission>> GetStudentSubmissions(string studentId)

	{

		return await _repo.GetStudentSubmissions(studentId);

	}

}

}