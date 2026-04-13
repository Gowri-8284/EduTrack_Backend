using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        //public async Task<EnrollmentAverageScoreDTO> GetAverageScoreAsync(string enrollmentId)

        //{

        //    if (string.IsNullOrEmpty(enrollmentId))

        //        throw new BadRequestException("EnrollmentId is required");

        //    var result = await _repo.GetAverageScoreAsync(enrollmentId);

        //    if (result == null)

        //        throw new NotFoundException("Enrollment not found");

        //    return result;

        //}

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
        public async Task<List<BatchCompletionDTO>> GetInstructorCompletionRate(string instructorId)

        {

            if (string.IsNullOrEmpty(instructorId))

                throw new ArgumentException("Instructor ID is required");

            var instructor = await _context.Instructor

                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);

            if (instructor == null)

                throw new NotFoundException("Instructor not found");

            var result = await _repo.GetBatchCompletionByInstructor(instructorId);

            if (result == null || result.Count == 0)

                throw new Exception("No student data found for this instructor");

            return result;

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
        public async Task<List<BatchClassCountDTO>> GetBatchClassCountsByInstructor(string instructorId)

        {

            // Step 1: Validation

            if (string.IsNullOrEmpty(instructorId))

                throw new ArgumentException("Instructor ID is required");

            // Step 2: Call repository

            var result = await _repo.GetBatchClassCountsByInstructor(instructorId);

            // Step 3: Optional check

            if (result == null || result.Count == 0)

                throw new Exception("No batches found for this instructor");

            return result;

        }
        public async Task<List<BatchStartDateDTO>> GetBatchStartDatesAsync()

        {

            var result = await _repo.GetBatchStartDatesAsync();

            if (result == null || result.Count == 0)

                throw new Exception("No batch data found");

            return result;

        }
        public async Task DeleteStudentAsync(string enrollmentId)

        {

            if (string.IsNullOrEmpty(enrollmentId))

                throw new ArgumentException("EnrollmentId required");

            await _repo.DeleteStudentAsync(enrollmentId);

        }
        public async Task UpdateStudentAsync(UpdateStudentDTO dto)

        {

            if (dto == null || string.IsNullOrEmpty(dto.EnrollmentId))

                throw new ArgumentException("Invalid data");

            await _repo.UpdateStudentAsync(dto);

        }
        public async Task GeneratePerformanceForBatch(string batchId)
        {
            // ✅ VALIDATION
            if (string.IsNullOrWhiteSpace(batchId))
            {
                throw new ArgumentException("BatchId cannot be null or empty");
            }
            try
            {
                // 👉 Call repository
                await _repo.GeneratePerformanceForBatch(batchId);
            }
            catch (Exception ex)
            {
                // ❗ LOG (optional)
                // Console.WriteLine(ex.Message);
                // ❗ THROW CUSTOM MESSAGE
                //throw new Exception("Error while generating performance for batch", ex);
                return;
            }
        }


        public async Task<double> GetCourseDropoutRateAsync(string courseId)
        {
            try
            {
                // ✅ Validation
                if (string.IsNullOrWhiteSpace(courseId))
                    throw new ArgumentException("CourseId cannot be empty");
                bool exists = await _context.Course
                    .AnyAsync(c => c.CourseId == courseId);
                if (!exists)
                    throw new KeyNotFoundException($"Course {courseId} not found");
                // ✅ Get data
                var rate = await _repo.GetCourseDropoutRateAsync(courseId);
                // ✅ Logical check
                if (rate < 0 || rate > 100)
                    throw new Exception("Invalid dropout rate");
                return rate;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while calculating course dropout rate", ex);
            }
        }






    }

}
