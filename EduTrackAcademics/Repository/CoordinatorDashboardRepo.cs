using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Repository

{

	public class CoordinatorDashboardRepo : ICoordinatorDashboardRepo
	{

		private readonly EduTrackAcademicsContext _context;


		public CoordinatorDashboardRepo(EduTrackAcademicsContext context)

		{

			_context = context;

		}


		public IEnumerable<object> GetPrograms() =>
	_context.Programs
		.Include(p => p.Qualification)
		.Select(p => new
		{
			p.ProgramId,
			p.ProgramName,
			Qualification = p.Qualification.QualificationName
		})
		.ToList();
		public IEnumerable<object> GetCourse() =>
		_context.Course
			.Select(c => new
			{
				c.CourseId,
				c.CourseName,
				c.Credits,
				c.DurationInWeeks
			})
			.ToList();


		public IEnumerable<object> GetAcademicYears(string programId) =>
		_context.AcademicYear
			.Where(y => y.ProgramId == programId)
			.Select(y => new
			{
				academicYearId = y.AcademicYearId,
				yearNumber = y.YearNumber,
				Display = "Year " + y.YearNumber   
			})
			.ToList();


		public Course AddCourse(CourseDTO dto)

		{
			string courseId = $"C{_context.Course.Count() + 1:D3}";

			var course = new Course
			{
				CourseId = courseId,
				CourseName = dto.CourseName,

				Credits = dto.Credits,

				DurationInWeeks = dto.DurationInWeeks,

				AcademicYearId = dto.AcademicYearId

			};


			_context.Course.Add(course);

			// ✅ ADD THIS (BatchConfig save)
			var config = new BatchConfig
			{
				CourseId = courseId,
				BatchSize = dto.BatchSize
			};

			_context.BatchConfigs.Add(config);

			_context.SaveChanges();

			return course;
		}
		public int GetBatchSize(string courseId)
		{
			var config = _context.BatchConfigs
				.FirstOrDefault(c => c.CourseId == courseId);

			return config?.BatchSize ?? 10; // default fallback
		}

		public IEnumerable<object> GetCourses(string yearId) =>
		_context.Course
			.Include(c => c.AcademicYear)
			.ThenInclude(a => a.Program).Where(c => c.AcademicYearId == yearId)
			.Select(c => new
			{
				c.CourseId,
				c.CourseName,
				c.Credits,
				c.DurationInWeeks,
				Year = c.AcademicYear.YearNumber,
				Program = c.AcademicYear.Program.ProgramName
			})
			
			.ToList();

		public IEnumerable<object> GetAllCourse() =>_context.Course
		.Select(c => new{
		c.CourseId,
		c.CourseName
		})

.ToList();
		public IEnumerable<object> GetStudents(string qualification, string program, int year) =>
			_context.Student
				.Where(s =>
					s.StudentQualification == qualification &&
					s.StudentProgram == program &&
					s.Year == year)
				.Select(s => new { s.StudentId, s.StudentName, s.StudentEmail })
				.ToList();
		public IEnumerable<object> GetStudentList() =>
	_context.Student
			.Select(s => new { s.StudentId, s.StudentName, s.StudentEmail })
		.ToList();


		public IEnumerable<object> GetInstructors(string skill) =>
		_context.Instructor

		.Where(i => i.InstructorSkills.Contains(skill))

		.Select(i => new { i.InstructorId, i.InstructorName, i.InstructorSkills })

		.ToList();


		public IEnumerable<object> GetBatches(string program, int year) =>
		_context.CourseBatches

		.Include(b => b.Course)

		.Where(b => _context.AcademicYear.Any(y =>
		y.AcademicYearId == b.Course.AcademicYearId &&
		y.YearNumber == year &&
		y.Program.ProgramName == program))

		.Select(b => new {

			b.BatchId,

			b.Course.CourseName,

			b.InstructorId,

			b.CurrentStudents,

			b.MaxStudents,

			b.IsActive

		})

		.ToList();


		public int GetBatchCount(string program, int year) =>
		_context.CourseBatches

		.Include(b => b.Course)

		.Count(b => _context.AcademicYear.Any(y =>
		y.AcademicYearId == b.Course.AcademicYearId &&
		y.YearNumber == year &&
		y.Program.ProgramName == program));


		public IEnumerable<object> GetStudentsInBatch(string batchId) =>
		_context.StudentBatchAssignments

		.Where(s => s.BatchId == batchId)

		.Select(s => new {

			s.Student.StudentId,

			s.Student.StudentName,

			s.Student.StudentEmail

		})

		.ToList();


		public object AssignBatches(AutoAssignBatchDTO dto)

		{

			var enrollments = _context.Enrollment

			.Include(e => e.Student)

			.Where(e =>
			e.CourseId == dto.CourseId &&
			e.Status == "Active" &&
			e.Student.StudentQualification == dto.Qualification &&
			e.Student.StudentProgram == dto.Program &&
			e.Student.Year == dto.Year)

			.Where(e =>
			!_context.StudentBatchAssignments.Any(a =>
			a.StudentId == e.StudentId))

			.OrderBy(e => e.Student.StudentId)

			.ToList();


			if (!enrollments.Any())

				return null;   // 🔥 changed (no exception)
			int batchCounter = _context.CourseBatches.Count() + 1;

			int assigned = 0;


			for (int i = 0; i < enrollments.Count; i += dto.BatchSize)

			{

				string batchId = $"B{batchCounter:D3}";


				var batch = new CourseBatch
				{

					BatchId = batchId,

					CourseId = dto.CourseId,

					InstructorId = dto.InstructorId,

					MaxStudents = dto.BatchSize,

					CurrentStudents = 0,

					IsActive = true
				};


				_context.CourseBatches.Add(batch);

				_context.SaveChanges();


				var group = enrollments.Skip(i).Take(dto.BatchSize).ToList();


				foreach (var enrollment in group)

				{

					_context.StudentBatchAssignments.Add(new StudentBatchAssignment
					{

						StudentId = enrollment.StudentId,

						BatchId = batchId
					});


					enrollment.Status = "Assigned";

					batch.CurrentStudents++;

					assigned++;

				}


				batch.IsActive = batch.CurrentStudents < batch.MaxStudents;

				_context.SaveChanges();


				batchCounter++;

			}


			return new
			{

				Message = "Batch assignment completed",

				AssignedStudents = assigned
			};

		}


		public object AssignSingleBatch(AutoAssignBatchDTO dto)

		{

			var students = _context.Enrollment

			.Include(e => e.Student)

			.Where(e =>
			e.CourseId == dto.CourseId &&
			e.Status == "Active" &&
			e.Student.StudentQualification == dto.Qualification &&
			e.Student.StudentProgram == dto.Program &&
			e.Student.Year == dto.Year)

			.Where(e =>
			!_context.StudentBatchAssignments.Any(a =>
			a.StudentId == e.StudentId))

			.OrderBy(e => e.Student.StudentId)

			.ToList();


			if (!students.Any())
				return new { Message = "No eligible students found" };

			// 🔑 Check for existing active batch with space
			var batch = _context.CourseBatches
				.FirstOrDefault(b => b.CourseId == dto.CourseId && b.IsActive);

			if (batch == null)
			{
				string batchId = $"B{_context.CourseBatches.Count() + 1:D3}";
				batch = new CourseBatch
				{
					BatchId = batchId,
					CourseId = dto.CourseId,
					InstructorId = dto.InstructorId,
					MaxStudents = dto.BatchSize,
					CurrentStudents = 0,
					IsActive = true
				};

				_context.CourseBatches.Add(batch);
				_context.SaveChanges();
			}

			var batchStudents = students.Take(dto.BatchSize - batch.CurrentStudents).ToList();

			foreach (var enrollment in batchStudents)

			{

				_context.StudentBatchAssignments.Add(new StudentBatchAssignment
				{

					StudentId = enrollment.StudentId,
					BatchId = batch.BatchId
				});


				enrollment.Status = "Assigned";

				batch.CurrentStudents++;

			}


			batch.IsActive = batch.CurrentStudents < batch.MaxStudents;

			_context.SaveChanges();


			return new
			{
				Message = "Batch updated/created & students assigned",
				BatchId = batch.BatchId,
				AssignedStudents = batchStudents.Count,

				RemainingStudents = students.Count - batchStudents.Count

			};

		}
		public Course? GetCourseById(string courseId)

		{

			return _context.Course.FirstOrDefault(c => c.CourseId == courseId);

		}


		public bool DeleteCourse(string courseId)

		{

			var course = _context.Course.FirstOrDefault(c => c.CourseId == courseId);


			if (course == null)

				return false;   // 🔥 changed (no exception)

			_context.Course.Remove(course);

			_context.SaveChanges();


			return true;

		}

		public object? UpdateCourse(string courseId, CourseDTO dto)

		{

			var course = _context.Course.FirstOrDefault(c => c.CourseId == courseId);


			if (course == null)

				return null;   // 🔥 only change
			course.CourseName = dto.CourseName;

			course.AcademicYearId = dto.AcademicYearId;

			course.Credits = dto.Credits;

			course.DurationInWeeks = dto.DurationInWeeks;


			_context.SaveChanges();


			return course;

		}


		public IEnumerable<object> GetInstructorBatches(string instructorId) =>
		_context.CourseBatches

		.Where(b => b.InstructorId == instructorId)

		.Select(b => new {

			b.BatchId,

			b.Course.CourseName,

			b.MaxStudents,

			b.CurrentStudents,

			b.IsActive

		})

		.ToList();


		public IEnumerable<object> InstructorDashboard(string instructorId) =>
		_context.CourseBatches

		.Where(b => b.InstructorId == instructorId)

		.Select(b => new {

			b.BatchId,

			Course = new { b.Course.CourseId, b.Course.CourseName },

			Students = _context.StudentBatchAssignments

		.Where(s => s.BatchId == b.BatchId)

		.Select(s => new { s.Student.StudentId, s.Student.StudentName })

								.ToList()

		})

						.ToList();

	}

}
