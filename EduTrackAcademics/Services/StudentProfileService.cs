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
		public async Task<StudentAdditionalDetailsDTO> GetAdditionalInfoAsync(string studentId)
		{
			// Check if student exists first using your existing repo method
			if (!await _repo.StudentExistsAsync(studentId))
			{
				throw new StudentNotFoundException($"Student with ID {studentId} not found.");
			}

			var details = await _repo.GetAdditionalInfoAsync(studentId);

			if (details == null)
			{
				// Return an empty DTO or throw exception based on your preference
				throw new StudentNotFoundException("Additional details have not been filled for this student.");
			}

			return details;
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
		public async Task<IEnumerable<(DateTime DueDate, string CourseName, string Status)>> GetAssignmentsForStudentAsync(string studentId)
		{
			var assignments = await _repo.GetStudentAssignmentsAsync(studentId);

			if (assignments == null || !assignments.Any())
			{
				// Return empty list or throw exception based on your frontend preference
				return Enumerable.Empty<(DateTime, string, string)>();
			}

			return assignments;
		}

	}
}
