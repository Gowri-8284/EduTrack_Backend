using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EduTrackAcademics.Services
{
	public class StudentProfileService : IStudentProfileService
	{
		private readonly IStudentProfileRepository _repo;
		public StudentProfileService(IStudentProfileRepository repo)
		{
			_repo = repo;
		}

		public async Task<List<StudentDTO>> GetAllStudentsAsync()
		{
			var result = await _repo.GetAllStudentsAsync();

			if (result == null || !result.Any())
			{
				throw new StudentNotFoundException("No students found");
			}
			return result;
		}

		public async Task<StudentDTO> GetPersonalInfoAsync(string studentId)
		{
			var result = await _repo.GetPersonalInfoAsync(studentId);

			if (result == null)
				throw new StudentNotFoundException("Student not found");
			return result;
		}
		public async Task<StudentDTO> GetProgramDetails(string studentId)
		{
			var result = await _repo.GetProgramDetailsAsync(studentId);

			if (result == null)
				throw new StudentNotFoundException("Student not found");
			return result;
		}
		public async Task UpdateAdditionalInfo(string studentId, StudentAdditionalDetailsDTO dto)
		{
			var exists = await _repo.StudentExistsAsync(studentId);

			if (!exists)
				throw new StudentNotFoundException("Student not found");
			await _repo.UpdateAdditionalInfoAsync(studentId, dto);
		}

		public async Task<int> GetCreditPointsAsync(string studentId)
		{
			// Validate student existence
			if (!await _repo.StudentExistsAsync(studentId))
			{
				throw new ArgumentException($"Student with ID {studentId} does not exist.");
			}

			// Fetch credits from repository

			var totalCredits = await _repo.GetCreditPointsAsync(studentId);
			return totalCredits;
		}

		//check whether student is enrolled in course before fetching assignment details


		// to get assignment due
		public async Task<(DateTime DueDate, string Type, string CourseName)> GetAssignmentDetailsForStudentAsync(string studentId, string courseId)
		{
			// Validate enrollment
			if (!await _repo.IsStudentEnrolledInCourseAsync(studentId, courseId))
			{
				throw new ArgumentException($"Student {studentId} is not enrolled in course {courseId}.");
			} // Fetch assignment details

			var assignment = await _repo.GetAssignmentDetailsAsync(courseId);

			if (assignment == null)
			{
				throw new InvalidOperationException($"No assignment found for course {courseId}.");
			}
			return assignment.Value;
		}

	}
}
