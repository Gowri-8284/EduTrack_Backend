using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;

using EduTrackAcademics.Exceptions;

using EduTrackAcademics.Model;

using EduTrackAcademics.Repository;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace EduTrackAcademics.Services


{


	public class PerformanceService : IPerformanceService

	{

		private readonly IPerformanceRepository _repo;

		private readonly EduTrackAcademicsContext _context;


		public PerformanceService(EduTrackAcademicsContext context, IPerformanceRepository repo)


		{

			_repo = repo;

			_context = context;


		}

            // ✅ COUNT
		public async Task<int> GetPerformanceCountAsync()


            {

                return await _repo.GetPerformanceCountAsync();


	}

            // ✅ AVERAGE SCORE
		public async Task<EnrollmentAverageScoreDTO> GetAverageScoreAsync(string enrollmentId)


            {

                if (string.IsNullOrEmpty(enrollmentId))


                    throw new BadRequestException("EnrollmentId is required");


	var result = await _repo.GetAverageScoreAsync(enrollmentId);


                if (result == null)


                    throw new NotFoundException("Enrollment not found");


                return result;


            }

// ✅ LAST UPDATED
public async Task<LastUpdatedDTO> GetLastUpdatedAsync(string enrollmentId)


{

	if (string.IsNullOrEmpty(enrollmentId))


		throw new BadRequestException("EnrollmentId is required");


	var result = await _repo.GetLastModifiedDateAsync(enrollmentId);


	if (result == null || result.StudentName == "No Data Found")


		throw new NotFoundException("No performance data found");


	return result;


}

// ✅ INSTRUCTOR BATCHES
public async Task<List<InstructorBatchDTO>> GetInstructorBatchesAsync(string instructorId)


{

	if (string.IsNullOrEmpty(instructorId))


		throw new BadRequestException("InstructorId is required");


	var result = await _repo.GetInstructorBatchesAsync(instructorId);


	if (result == null || !result.Any())


		throw new NotFoundException("No batches found for instructor");


	return result;


}

// ✅ BATCH REPORT
public async Task<GetBatchReportDTO> GetBatchReportAsync(string batchId)


{

	if (string.IsNullOrEmpty(batchId))


		throw new BadRequestException("BatchId is required");


	var result = await _repo.GetBatchPerformanceAsync(batchId);


	if (result == null)


		//  throw new NotFoundException("Batch not found");
		return null;


	return result;


}
public async Task<double> GetInstructorCompletionRate(string instructorId)


{

	// 🔥 VALIDATION
	if (string.IsNullOrEmpty(instructorId))


	throw new ArgumentException("Instructor ID is required");


	// 🔥 CHECK instructor exists
	var instructor = await _context.Instructor

	.Where(i=>i.InstructorId==instructorId)           .AsQueryable()                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);


	if (instructor == null)


		throw new NotFoundException("Instructor not found");


	try

	{

		var result = await _repo.GetInstructorCompletionRate(instructorId);


		// 🔥 EDGE CASE
		if (result == 0)


		throw new Exception("No student data found for this instructor");


		return result;


	}

	catch (Exception ex)


	{

		// 🔥 LOG (optional)

		Console.WriteLine(ex.Message);

		throw;


	}

}
public async Task<object> GetOngoingBatches(string instructorId)


{


	// 🔥 VALIDATION
	if (string.IsNullOrWhiteSpace(instructorId))


	throw new ArgumentException("Instructor ID is required");


            // 🔥 CHECK instructor exists
			var instructor = await _context.Instructor


                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);


	if (instructor == null)


		throw new NotFoundException("Instructor not found");


	try


	{


		var batches = await _repo.GetBatchesByInstructor(instructorId);


		// 🔥 FILTER ONGOING
		var ongoing = batches.Where(b => b.IsActive).ToList();


		return new


		{


			count = ongoing.Count,


			batchIds = ongoing.Select(b => b.BatchId).ToList()


		};


	}


	catch (Exception ex)


	{


		Console.WriteLine(ex.Message); // optional log
		throw new Exception("Error fetching ongoing batches");


	}


}

public async Task<List<InstructorBatchDTO>> GetAllBatchesAsync()


{


	try


	{


		var result = await _repo.GetAllBatchesAsync();


		// ✅ Validation
		if (result == null || !result.Any())


		{


			throw new NotFoundException("No batches found");


		}


		return result;


	}


	catch (Exception ex)


	{


		// optional: log error
	throw new Exception("Error while fetching all batches: " + ex.Message);


	}


}






    }


}