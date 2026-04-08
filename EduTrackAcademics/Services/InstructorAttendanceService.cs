using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Humanizer;
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


		var enrollment = await _context.Enrollment


	  .FirstOrDefaultAsync(e => e.EnrollmentId == dto.EnrollmentID);



      if (enrollment == null)


        throw new ApplicationException("Invalid Enrollment ID");



  // 2️⃣ Find the actual batch the student belongs to for this course
  var assignment = await _context.StudentBatchAssignments


    .Include(s => s.CourseBatch)
	

.FirstOrDefaultAsync(sba => sba.StudentId == enrollment.StudentId


 && sba.CourseBatch.CourseId == enrollment.CourseId);



      if (assignment == null)


        throw new ApplicationException("Student is not assigned to any batch for this course");



		// 3️⃣ Use the correct batch ID
		var batchId = assignment.BatchId;



		// 4️⃣ Create attendance
		var AttendanceId = await _repo.GenerateAttendanceIdAsync();



		var attendance = new Attendance
		{


			AttendanceID = AttendanceId,


			EnrollmentID = dto.EnrollmentID,


			BatchId = batchId,           // ✅ correct batch           
		 StudentId = enrollment.StudentId, // ✅ correct student          
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

			var attendanceList = new List<Attendance>();


			var last = await _repo.GetLastAttendanceAsync();

			int counter = last == null ? 0 : int.Parse(last.AttendanceID.Substring(2));


			foreach (var student in dto.Students)

			{

				var enrollment = await _context.Enrollment

				.FirstOrDefaultAsync(e => e.EnrollmentId == student.EnrollmentID);


				if (enrollment == null)

					throw new ApplicationException($"Invalid EnrollmentID: {student.EnrollmentID}");


				var assignment = await _context.StudentBatchAssignments

				.Include(s => s.CourseBatch)

				.FirstOrDefaultAsync(sba => sba.StudentId == enrollment.StudentId

				 && sba.CourseBatch.CourseId == enrollment.CourseId);


				if (assignment == null)

					throw new ApplicationException($"Student {enrollment.StudentId} is not assigned to any batch for this course");


				var alreadyMarked = await _repo.AttendanceExistsAsync(student.EnrollmentID, dto.SessionDate);

				if (alreadyMarked)

					throw new ApplicationException($"Attendance already marked for {student.EnrollmentID}");


				counter++;

				var attendanceId = $"AT{counter:D3}";


				attendanceList.Add(new Attendance
				{

					AttendanceID = attendanceId,

					EnrollmentID = student.EnrollmentID,

					BatchId = assignment.BatchId,
					StudentId = enrollment.StudentId,
					SessionDate = dto.SessionDate,

					Mode = dto.Mode,

					Status = student.Status,

					IsDeleted = false
				});

			}


			await _repo.AddBatchAttendanceAsync(attendanceList);


			return "Batch attendance marked successfully";

		}


		public async Task<List<string>> GetEnrollmentIdsByBatchAsync(string batchId)

		{

			return await _repo.GetEnrollmentsByBatchAsync(batchId);

		}



		public async Task<List<object>> GetAllAttendanceAsync()

		{

			var attendances = await _repo.GetAllAttendanceAsync();


			return attendances.Select(a => new {

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


			return attendances.Select(a => new {

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


			return attendances.Select(a => new {

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


			return attendances.Select(a => new {

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




		public async Task<string> PatchAttendanceStatusAsync(string attendanceId, string enrollmentId, string status)

		{

			var attendance =

			await _repo.GetAttendanceByIdAsync(attendanceId);


			if (attendance == null)

				throw new ApplicationException("Attendance not found");



			if (attendance.EnrollmentID != enrollmentId)

				throw new ApplicationException(

				"EnrollmentID does not match the attendance record");


			attendance.Status = status;

			attendance.UpdatedOn = DateTime.Now;


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


	}

}