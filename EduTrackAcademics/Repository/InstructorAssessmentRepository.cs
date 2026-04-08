using EduTrackAcademics.Data;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Repository

{

	public class InstructorAssessmentRepository : IInstructorAssessmentRepository
	{

		private readonly EduTrackAcademicsContext _context;


		public InstructorAssessmentRepository(EduTrackAcademicsContext context)

		{

			_context = context;

		}


    // ASSESSMENT
	public async Task<string> GenerateAssessmentIdAsync()

    {

      var last = await _context.Assessments

.OrderByDescending(a => a.AssessmentID)

.Select(a => a.AssessmentID)

.FirstOrDefaultAsync();


		int next = last == null ? 1 : int.Parse(last.Substring(1)) + 1;

      return $"A{next:000}";


    }


    public async Task AddAssessmentAsync(Assessment assessment)

		{

			await _context.Assessments.AddAsync(assessment);

			await _context.SaveChangesAsync();

		}


		public async Task<List<Assessment>> GetAllAssessmentsAsync()

		{

			return await _context.Assessments.ToListAsync();

		}


		public async Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date)

		{

			return await _context.Assessments

			.Where(a => a.DueDate.Date == date.Date)

			.ToListAsync();

		}


		public async Task<Assessment?> GetAssessmentByIdAsync(string id)

		=> await _context.Assessments.FindAsync(id);


		public async Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId)

		=> await _context.Assessments

		.Where(a => a.CourseId == courseId)

		.ToListAsync();


		public async Task UpdateAssessmentAsync(Assessment assessment)

		{

			_context.Assessments.Update(assessment);

			await _context.SaveChangesAsync();

		}


		public async Task DeleteAssessmentAsync(Assessment assessment)

		{

			_context.Assessments.Remove(assessment);

			await _context.SaveChangesAsync();


		}



    //public async Task<Submission> GetSubmissionAsync(string studentId, string assessmentId)//{//  return await _context.Submissions//    .FirstOrDefaultAsync(s =>//      s.StudentID == studentId &&//      s.AssessmentId == assessmentId);//}//public async Task<int> GetTotalMarksAsync(string assessmentId)//{//  return await _context.Questions//    .Where(q => q.AssessmentId == assessmentId)//    .SumAsync(q => q.Marks);//}//public async Task UpdateSubmissionAsync(Submission submission)//{//  _context.Submission.Update(submission);//  await _context.SaveChangesAsync();//}// QUESTIONS
	public async Task<string> GenerateQuestionIdAsync()

    {

      var last = await _context.Questions

.OrderByDescending(q => q.QuestionId)

.Select(q => q.QuestionId)

.FirstOrDefaultAsync();


		int next = last == null ? 1 : int.Parse(last.Substring(1)) + 1;

      return $"Q{next:000}";

    }



public async Task<bool> QuestionExistsAsync(string id)

=> await _context.Questions.AnyAsync(q => q.QuestionId == id);


public async Task AddQuestionAsync(Question question)

{

	await _context.Questions.AddAsync(question);

	await _context.SaveChangesAsync(); // <-- critical    
}



public async Task<Question?> GetQuestionByIdAsync(string id)

=> await _context.Questions.FindAsync(id);


public async Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId)

=> await _context.Questions

.Where(q => q.AssessmentId == assessmentId)

.ToListAsync();


public async Task UpdateQuestionAsync(Question question)

{

	_context.Questions.Update(question);

	await _context.SaveChangesAsync();

}


public async Task DeleteQuestionAsync(Question question)

{

	_context.Questions.Remove(question);

	await _context.SaveChangesAsync();

}


// fetching data
public async Task<Submission> GetSubmission(string studentId, string assessmentId)

{

	return await _context.Submission

	.FirstOrDefaultAsync(s => s.StudentID == studentId && s.AssessmentId == assessmentId);

}


public async Task<int> GetTotalMarks(string assessmentId)

{

	return await _context.Questions

	.Where(q => q.AssessmentId == assessmentId)

	.SumAsync(q => q.Marks);

}


public async Task<List<Submission>> GetStudentSubmissions(string studentId)

{

	return await _context.Submission

	.Where(s => s.StudentID == studentId)

	.ToListAsync();

}

  }

}