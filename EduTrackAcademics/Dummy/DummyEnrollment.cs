//using EduTrackAcademics.Model;

//namespace EduTrackAcademics.Dummy
//{
//	public class DummyEnrollment : Enrollment
//	{
//		public List<Enrollment> Enrollments = new List<Enrollment>
//		{
//			new Enrollment
//			{
//				EnrollmentId = "en_001",
//				EnrollmentDate = new DateTime(2026, 1, 10),
//				Status = "Active",
//				credits = 0,
//				StudentId = "st_101",
//				CourseId = "c_01"
//			},
//			new Enrollment
//			{
//				EnrollmentId = "en_002",
//				EnrollmentDate = new DateTime(2026, 1, 15),
//				Status = "Completed",
//				credits = 3,
//				StudentId = "st_102",
//				CourseId = "c_01"
//			},
//			new Enrollment
//			{
//				EnrollmentId = "en_003",
//				EnrollmentDate = new DateTime(2026, 2, 01),
//				Status = "Dropped",
//				credits = 0,
//				StudentId = "st_103",
//				CourseId = "c_02"
//			},
//			new Enrollment
//			{
//				EnrollmentId = "en_004",
//				EnrollmentDate = DateTime.Now.AddDays(-5),
//				Status = "Active",
//				credits = 0,
//				StudentId = "st_101",
//				CourseId = "c_03"
//			}
//		};

//		public List<Module> Modules = new List<Module>
//		{
//			new Module {
//				ModuleID = "M_101",
//				CourseID = "c_01",
//				Name = "Getting Started with MVC",
//				SequenceOrder = 1
//			},
//			new Module {
//				ModuleID = "M_102",
//				CourseID = "c_01",
//				Name = "Entity Framework Basics",
//				SequenceOrder = 2
//			}
//		};

//		public List<Content> Contents = new List<Content>
//		{
//        // Contents for Module M_101
//			new Content {
//				ContentID = "C_001", ModuleID = "M_101", Title = "MVC Pattern Video",
//				ContentType = "Video", ContentURI = "/videos/mvc-intro.mp4"
//			},
//			new Content {
//				ContentID = "C_002", ModuleID = "M_101", Title = "Architecture PDF",
//				ContentType = "PDF", ContentURI = "/docs/arch.pdf"
//			},

//        // Contents for Module M_102
//			new Content {
//				ContentID = "C_003", ModuleID = "M_102", Title = "DB Connection Guide",
//				ContentType = "PDF", ContentURI = "/docs/db-guide.pdf"
//			}
//		};

//		public List<StudentProgress> StudentProgress = new List<StudentProgress>
//		{
//			new StudentProgress
//			{
//				ProgressID = "sp_001",
//				StudentId = "st_101",
//				CourseId = "c_01",
//				ContentId = "C_001",
//				IsCompleted = true,
//				CompletionDate = DateTime.Now.AddDays(-2)
//			},
//			new StudentProgress
//			{
//				ProgressID = "sp_002",
//				StudentId = "st_101",
//				CourseId = "c_01",
//				ContentId = "C_002",
//				IsCompleted = true,
//				CompletionDate = DateTime.Now.AddDays(-1)
//			}

//		};

//		public List<Assessment> Assessments = new List<Assessment>
//		{
//			new Assessment
//			{
//				AssessmentID="AS001",
//				CourseID="c_01",
//				Type="Assignment",
//				MaxMarks=20,
//				DueDate=DateTime.Today.AddDays(5),
//				Status="Open"
//			},
//			new Assessment
//			{
//				AssessmentID="AS002",
//				CourseID="c_01",
//				Type="Quiz",
//				MaxMarks=10,
//				DueDate=DateTime.Today.AddDays(2),
//				Status="Open"
//			},
//			new Assessment
//			{
//				AssessmentID="AS003",
//				CourseID="c_02",
//				Type="Exam",
//				MaxMarks=100,
//				DueDate=DateTime.Today.AddDays(10),
//				Status="Closed"
//			}
//		};
//	}
//}
