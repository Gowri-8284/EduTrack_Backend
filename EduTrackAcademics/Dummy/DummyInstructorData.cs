using EduTrackAcademics.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Dummy
{
	public class DummyInstructorData
	{

		public static List<CourseBatch> GetBatches()
		{
			return new List<CourseBatch>
			{
				new CourseBatch
				{
					BatchId = "B001",
					CourseId = "CSE101",
					InstructorId = "I001",
					MaxStudents = 30,
					CurrentStudents = 2,
					IsActive = true
				}
			};
		}
		// =========================
		// STUDENTS IN BATCH
		// =========================
		public static List<StudentBatchAssignment> GetStudents()
		{
			return new List<StudentBatchAssignment>
			{
				new StudentBatchAssignment
				{
					BatchId = "B001",
					Student = new Student
					{
						StudentId = "S001",
						StudentName = "Rahul",
						StudentEmail = "rahul@gmail.com"
					}
				},
				new StudentBatchAssignment
				{
					BatchId = "B001",
					Student = new Student
					{
						StudentId = "S002",
						StudentName = "Anjali",
						StudentEmail = "anjali@gmail.com"
					}
				}
			};
		}

		// =========================
		// MODULES
		// =========================
		public static List<Module> GetModules()
		{
			return new List<Module>
			{
				new Module
				{
					ModuleID = "M001",
					CourseId = "CSE101",
					Name = "Introduction to C#",
					SequenceOrder = 1,
					LearningObjectives = "Basics of C#"
				},
				new Module
				{
					ModuleID = "M002",
					CourseId = "CSE101",
					Name = "OOP Concepts",
					SequenceOrder = 2,
					LearningObjectives = "Classes & Objects"
				}
			};
		}

		// =========================
		// CONTENT
		// =========================
		public static List<Content> GetContents()
		{
			return new List<Content>
			{
				new Content
				{
					ContentID = "CT001",
					ModuleID = "M001",
					ContentType = "Video",
					Title = "C# Introduction",
					ContentURI = "https://example.com/video",
					Status = "Published"
				},
				new Content
				{
					ContentID = "CT002",
					ModuleID = "M001",
					ContentType = "PDF",
					Title = "C# Notes",
					ContentURI = "https://example.com/pdf",
					Status = "Published"
				}
			};
		}

		// =========================
		// ASSESSMENTS + QUESTIONS
		// =========================
		public static List<Assessment> GetAssessments()
		{
			return new List<Assessment>
			{
				new Assessment
				{
					AssessmentID = "A001",
					CourseId = "CSE101",
					Type = "Quiz",
					MaxMarks = 20,
					DueDate = DateTime.Today.AddDays(5),
					Status = "Open",
					Questions = new List<Question>
					{
						new Question
						{
							QuestionId="Q1",
							QuestionText="What is C#?",
							QuestionType="MCQ",
							OptionA="Language",
							OptionB="OS",
							OptionC="Browser",
							OptionD="Hardware",
							CorrectOption="Language",
							Marks=5
						},
						new Question
						{
							QuestionId="Q2",
							QuestionText="C# is OOP?",
							QuestionType="TRUE/FALSE",
							CorrectOption="True",
							Marks=5
						}
					}
				}
			};
		}

		// =========================
		// ATTENDANCE
		// =========================
		public static List<Attendance> Attendances = new List<Attendance>();
	}
}
	
	//	[HttpGet("instructors")]
	//	public IActionResult GetInstructors(string skill)
	//	{
	//		var instructors = _context.Instructor
	//			.Where(i => i.InstructorSkills.Contains(skill))
	//			.Select(i => new
	//			{
	//				i.InstructorId,
	//				i.InstructorName,
	//				i.InstructorSkills
	//			})
	//			.ToList();

	//		return Ok(instructors);
	//	}
	//}

