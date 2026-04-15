using System.Diagnostics.Metrics;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EduTrackAcademics.Services
{
	public class InstructorAttendanceService : IInstructorAttendanceService
	{
		private readonly IInstructorAttendanceRepository _repo;

		private readonly EduTrackAcademicsContext _context;

		public InstructorAttendanceService(IInstructorAttendanceRepository repo, EduTrackAcademicsContext context)
		{
			_repo = repo;
			_context = context;
		}

		// ATTENDANCE

		public async Task<string> MarkAttendanceAsync(AttendanceDTO dto)

		{

			var enrollmentExists = await _repo.EnrollmentExistsAsync(dto.EnrollmentID);

			if (!enrollmentExists)

				throw new ApplicationException("Invalid Enrollment ID");


			var batchExists = await _repo.BatchExistsAsync(dto.BatchId);

			if (!batchExists)

				throw new ApplicationException("Invalid Batch ID");


			var alreadyMarked = await _repo.AttendanceExistsAsync(dto.EnrollmentID, dto.SessionDate);

			if (alreadyMarked)

				throw new ApplicationException("Attendance already marked for this date");


			var attendanceId = await _repo.GenerateAttendanceIdAsync();


			//  Get enrollment
			var enrollment = await _context.Enrollment

		  .FirstOrDefaultAsync(e => e.EnrollmentId == dto.EnrollmentID);


			if (enrollment == null)

				throw new ApplicationException("Invalid Enrollment ID");


			//  Find the actual batch the student belongs to for this course
			var assignment = await _context.StudentBatchAssignments

		  .Include(s => s.CourseBatch)

		  .FirstOrDefaultAsync(sba => sba.StudentId == enrollment.StudentId

		   && sba.CourseBatch.CourseId == enrollment.CourseId);


			if (assignment == null)

				throw new ApplicationException("Student is not assigned to any batch for this course");


			//  Use the correct batch ID
			var batchId = assignment.BatchId;


			//  Create attendance
			var AttendanceId = await _repo.GenerateAttendanceIdAsync();


			var attendance = new Attendance
			{

				AttendanceID = AttendanceId,

				EnrollmentID = dto.EnrollmentID,

				BatchId = batchId,           //  correct batch    
				StudentId = enrollment.StudentId, //  correct student    
				SessionDate = dto.SessionDate,

				Mode = dto.Mode,

				Status = dto.Status,

				IsDeleted = false
			};


			await _repo.AddAttendanceAsync(attendance);


			return $"Attendance marked successfully with ID {AttendanceId}";

		}

		public async Task<List<BatchDropdownDTO>> GetAllBatchesAsync()
		{
			var batchIds = await _repo.GetAllBatchIdsAsync();
			return batchIds.Select(b => new BatchDropdownDTO { BatchId = b }).ToList();
		}

		public async Task<string> MarkBatchAttendanceAsync(MarkBatchAttendanceDTO dto)
		{
			// Validate Batch
			var batch = await _context.CourseBatches
				.FirstOrDefaultAsync(b => b.BatchId == dto.BatchId);

			if (batch == null)
				return "Batch not found";

			// Check Active Condition
			if (batch.MaxStudents != batch.CurrentStudents)
			{
				batch.IsActive = false;
				return "Course is inactive. Please fill all students to activate batch.";
			}

			// Activate Batch if Full
			if (!batch.IsActive)
			{
				batch.IsActive = true;
				batch.LastFilledDate = DateTime.Now;

				_context.CourseBatches.Update(batch);
				await _context.SaveChangesAsync();
			}

			// Generate Base Counter (FIX FOR YOUR ERROR 🔥)
			var lastAttendance = await _context.Attendances
				.OrderByDescending(a => a.AttendanceID)
				.FirstOrDefaultAsync();

			int counter = lastAttendance == null
				? 1
				: int.Parse(lastAttendance.AttendanceID.Substring(2)) + 1;

			var attendanceList = new List<Attendance>();

			// Loop Students
			foreach (var student in dto.Students)
			{
				//  Validate Enrollment
				var enrollment = await _context.Enrollment
					.FirstOrDefaultAsync(e => e.EnrollmentId == student.EnrollmentID);

				if (enrollment == null)
					return $"Invalid EnrollmentID: {student.EnrollmentID}";

				//  Validate Student-Batch Mapping (VERY IMPORTANT)
				var isMapped = await _context.StudentBatchAssignments
					.AnyAsync(x =>
						x.StudentId == enrollment.StudentId &&
						x.BatchId == dto.BatchId);

				if (!isMapped)
					return $"Student not assigned to this batch: {student.EnrollmentID}";

				//  Check duplicate attendance
				var alreadyExists = await _context.Attendances
					.AnyAsync(a =>
						a.EnrollmentID == student.EnrollmentID &&
						a.SessionDate.Date == dto.SessionDate.Date);

				if (alreadyExists)
					return $"Attendance already marked for {student.EnrollmentID}";

				//  Generate UNIQUE AttendanceID (🔥 FIX)
				var attendanceId = $"AT{counter:D3}";
				counter++;

				//  Create attendance record
				attendanceList.Add(new Attendance
				{
					AttendanceID = attendanceId,
					EnrollmentID = student.EnrollmentID,
					StudentId = enrollment.StudentId,
					BatchId = dto.BatchId,
					SessionDate = dto.SessionDate,
					Mode = dto.Mode,
					Status = student.Status,
					IsDeleted = false
				});
			}

			// Save to DB
			await _context.Attendances.AddRangeAsync(attendanceList);
			await _context.SaveChangesAsync();

			return "Batch attendance marked successfully";
		}

		public async Task<List<BatchStudentsDTO>> GetStudentsForAttendanceAsync(string batchId)
		{
			//  Validate Batch
			var batch = await _repo.GetBatchByIdAsync(batchId);

			if (batch == null)
				throw new Exception("Batch not found");

			//  Check Active Status (IMPORTANT CONDITION)
			if (!batch.IsActive)
				throw new Exception("Batch is inactive. Cannot mark attendance.");

			//  Get Students (Enrollment Data)
			var enrollments = await _repo.GetEnrollmentsByBatchIdAsync(batchId);
			var distinctStudents = enrollments
									.GroupBy(e => e.StudentId)
									.Select(g => g.OrderByDescending(x => x.EnrollmentDate)
												.First())
									.ToList();

			//  Map to DTO
			var result = distinctStudents.Select(e => new BatchStudentsDTO
			{
				EnrollmentId = e.EnrollmentId,
				StudentId = e.StudentId,
				StudentName = e.Student.StudentName
			}).ToList();

			return result;
		}

		public async Task<List<AttendanceSummaryDTO>> GetAttendanceSummaryAsync()
		{
			var data = await _repo.GetAllAttendanceAsync();

			var grouped = data
				.GroupBy(a => new { a.BatchId, a.SessionDate.Date })
				.Select(g => {
					var firstRecord = g.First();

					return new AttendanceSummaryDTO
					{
						BatchId = g.Key.BatchId,
						SessionDate = g.Key.Date,
						CourseId = firstRecord.Enrollment.CourseId,
						CourseName = firstRecord.Enrollment.Course?.CourseName ?? "N/A",

						TotalStudents = g.Count(), 
						PresentCount = g.Count(x => x.Status == "Present"),
						AbsentCount = g.Count(x => x.Status == "Absent")
					};
				})
				.OrderByDescending(x => x.SessionDate)
				.ToList();

			return grouped;
		}
		
		public async Task<List<AttendanceDetailsDTO>> GetAttendanceDetailsAsync(string batchId, DateTime date)
		{
			var data = await _repo.GetAttendanceByBatchAndDateAsync(batchId, date);

			var result = data.Select(a => new AttendanceDetailsDTO
			{
				AttendanceId = a.AttendanceID,
				StudentId = a.StudentId,
				StudentName = a.Enrollment?.Student?.StudentName ?? "N/A",
				EnrollmentID = a.EnrollmentID,
				Status = a.Status
			}).ToList();

			return result;
		}


		public async Task<List<string>> GetEnrollmentIdsByBatchAsync(string batchId)
		{
			return await _repo.GetEnrollmentsByBatchAsync(batchId);
		}


		public async Task<List<object>> GetAllAttendanceAsync()
		{
			var attendances = await _repo.GetAllAttendanceAsync();

			return attendances.Select(a => new
			{
				a.AttendanceID,
				a.EnrollmentID,
				StudentName = a.Enrollment.Student.StudentName,
				CourseName = a.Enrollment.Course.CourseName,
				a.BatchId,
				a.SessionDate,
				a.Mode,
				a.Status
			}).ToList<object>();
		}

		public async Task<List<object>> GetAttendanceByDateAsync(DateTime date)
		{
			var attendances = await _repo.GetAttendanceByDateAsync(date);

			return attendances.Select(a => new
			{
				a.AttendanceID,
				a.EnrollmentID,
				StudentName = a.Enrollment.Student.StudentName,
				CourseName = a.Enrollment.Course.CourseName,
				a.BatchId,
				a.SessionDate,
				a.Mode,
				a.Status
			}).ToList<object>();
		}

		public async Task<List<object>> GetAttendanceByBatchAsync(string batchId)
		{
			var attendances = await _repo.GetAttendanceByBatchAsync(batchId);

			return attendances.Select(a => new
			{
				a.AttendanceID,
				a.EnrollmentID,
				StudentName = a.Enrollment.Student.StudentName,
				CourseName = a.Enrollment.Course.CourseName,
				a.BatchId,
				a.SessionDate,
				a.Mode,
				a.Status
			}).ToList<object>();
		}

		public async Task<List<object>> GetAttendanceByEnrollmentAsync(string enrollmentId)
		{
			var attendances = await _repo.GetAttendanceByEnrollmentAsync(enrollmentId);

			return attendances.Select(a => new
			{
				a.AttendanceID,
				a.EnrollmentID,
				StudentName = a.Enrollment.Student.StudentName,
				CourseName = a.Enrollment.Course.CourseName,
				a.BatchId,
				a.SessionDate,
				a.Mode,
				a.Status
			}).ToList<object>();
		}

		public async Task<string> UpdateAttendanceAsync(string attendanceId, AttendanceDTO dto)
		{
			var attendance = await _repo.GetAttendanceByIdAsync(attendanceId);

			if (attendance == null)
				throw new ApplicationException("Attendance not found");

			attendance.EnrollmentID = dto.EnrollmentID;
			attendance.BatchId = dto.BatchId;
			attendance.SessionDate = dto.SessionDate;
			attendance.Mode = dto.Mode;
			attendance.Status = dto.Status;
			attendance.UpdatedOn = DateTime.Now;

			await _repo.UpdateAttendanceAsync(attendance);

			return $"Attendance {attendanceId} updated successfully";
		}

		public async Task<string> PatchAttendanceStatusAsync(string attendanceId, string status)
		{
			//  Validate
			if (string.IsNullOrWhiteSpace(status))
				throw new ApplicationException("Status is required");

			if (status != "Present" && status != "Absent")
				throw new ApplicationException("Status must be Present or Absent");

			//  Get Attendance
			var attendance = await _repo.GetAttendanceByIdAsync(attendanceId);

			if (attendance == null)
				throw new ApplicationException("Attendance not found");

			//  Update only Status
			attendance.Status = status;
			attendance.UpdatedOn = DateTime.Now;

			//  Save
			await _repo.UpdateAttendanceAsync(attendance);

			return $"Attendance {attendanceId} updated successfully";
		}

		public async Task<string> DeleteAttendanceAsync(string attendanceId, string reason)
		{
			var attendance = await _repo.GetAttendanceByIdAsync(attendanceId);

			if (attendance == null)
				return "Attendance not found or already deleted";

			attendance.IsDeleted = true;
			attendance.DeletionReason = reason;
			attendance.DeletionDate = DateTime.Now;

			await _repo.SoftDeleteAttendanceAsync(attendance);

			return $"Attendance {attendanceId} deleted successfully";
		}

		//  DELETE BY BATCH
		public async Task<string> DeleteAttendanceByBatchAsync(string batchId, DateTime date, string reason)
		{
			var records = await _repo.GetAttendanceByBatchAndDateAsync(batchId, date);

			if (records == null || records.Count == 0)
				return "No attendance records found for this batch and date.";

			await _repo.DeleteBatchAttendanceAsync(records, reason);

			return "Batch attendance deleted successfully.";
		}

		//  DELETE BY COURSE
		public async Task<string> DeleteAttendanceByCourseAsync(string courseId, DateTime date, string reason)
		{
			var records = await _repo.GetAttendanceByCourseAndDateAsync(courseId, date);

			if (records == null || records.Count == 0)
				return "No attendance records found for this course and date.";

			await _repo.DeleteBatchAttendanceAsync(records, reason);

			return "Course attendance deleted successfully.";
		}

		public async Task<string> RestoreAttendanceByBatchAsync(string batchId, DateTime date)
		{
			var records = await _repo.GetDeletedAttendanceByBatchAndDateAsync(batchId, date);

			if (records == null || records.Count == 0)
				return "No deleted attendance records found.";

			await _repo.RestoreBatchAttendanceAsync(records);

			return "Attendance restored successfully.";
		}

		public async Task<List<AttendanceSummaryDTO>> GetDeletedAttendanceSummaryAsync()
		{
			var data = await _repo.GetDeletedAttendancesAsync();

			var grouped = data
				.GroupBy(a => new { a.BatchId, a.SessionDate.Date })
				.Select(g => new AttendanceSummaryDTO
				{
					BatchId = g.Key.BatchId,
					SessionDate = g.Key.Date,
					CourseName = g.First().Enrollment.CourseId,

					TotalStudents = g.Count(),
					PresentCount = g.Count(x => x.Status == "Present"),
					AbsentCount = g.Count(x => x.Status == "Absent")
				})
				.OrderByDescending(x => x.SessionDate)
				.ToList();

			return grouped;
		}

		public IEnumerable<GetCourseResponseDTO> GetAllCoursesByInstructorId(string instructorId)
		{
			return _repo.GetAllCoursesByInstructorId(instructorId);
		}

		public CourseBatchDTO GetBatchByCourse(string? courseId, string courseName)
		{
			return _repo.GetBatchByCourse(courseId, courseName);
		}
	}
}
