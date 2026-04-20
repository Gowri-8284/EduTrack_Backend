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

        //count of the students
        public async Task<int> GetPerformanceCountAsync()
        {
            return await _repo.GetPerformanceCountAsync();
        }

        //Last Updated of the student details
        public async Task<LastUpdatedDTO> GetLastUpdatedAsync(string enrollmentId)

        {

            if (string.IsNullOrEmpty(enrollmentId))

                throw new BadRequestException("EnrollmentId is required");

            var result = await _repo.GetLastModifiedDateAsync(enrollmentId);

            if (result == null || result.StudentName == "No Data Found")

                throw new NotFoundException("No performance data found");

            return result;

        }

        // Instrcutor assigned batches
        public async Task<List<InstructorBatchDTO>> GetInstructorBatchesAsync(string instructorId)

        {

            if (string.IsNullOrEmpty(instructorId))

                throw new BadRequestException("InstructorId is required");

            var result = await _repo.GetInstructorBatchesAsync(instructorId);

            if (result == null || !result.Any())

                throw new NotFoundException("No batches found for instructor");

            return result;

        }

        // Report for a batch
        public async Task<GetBatchReportDTO> GetBatchReportAsync(string batchId)

        {

            if (string.IsNullOrEmpty(batchId))

                throw new BadRequestException("BatchId is required");

            var result = await _repo.GetBatchPerformanceAsync(batchId);

            if (result == null)

                return null;

            return result;

        }
        //get completion rate of the students in a batch
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

        //get ongoing batches of the instructor
        public async Task<object> GetOngoingBatches(string instructorId)
        {
            if (string.IsNullOrWhiteSpace(instructorId))
                throw new ArgumentException("Instructor ID is required");
            var instructor = await _context.Instructor
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);
            if (instructor == null)
                throw new NotFoundException("Instructor not found");
            try
            {
                var batches = await _repo.GetBatchesByInstructor(instructorId);
                var ongoing = batches.Where(b => b.IsActive).ToList();
                return new
                {
                    count = ongoing.Count,
                    batchIds = ongoing.Select(b => b.BatchId).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                throw new Exception("Error fetching ongoing batches");
            }
        }

        //get all batches of the instructor
        public async Task<List<InstructorBatchDTO>> GetAllBatchesAsync()
        {
            try
            {
                var result = await _repo.GetAllBatchesAsync();
                if (result == null || !result.Any())
                {
                    throw new NotFoundException("No batches found");
                }
                return result;
            }

            catch (Exception ex)
            {
                throw new Exception("Error while fetching all batches: " + ex.Message);
            }
        }

        //get count of the classess
        public async Task<List<BatchClassCountDTO>> GetBatchClassCountsByInstructor(string instructorId)

        {


            if (string.IsNullOrEmpty(instructorId))

                throw new ArgumentException("Instructor ID is required");

            var result = await _repo.GetBatchClassCountsByInstructor(instructorId);

            if (result == null || result.Count == 0)

                throw new Exception("No batches found for this instructor");

            return result;

        }

        //get batch start dates and end dates
        public async Task<List<BatchStartDateDTO>> GetBatchStartDatesAsync()

        {

            var result = await _repo.GetBatchStartDatesAsync();

            if (result == null || result.Count == 0)

                throw new Exception("No batch data found");

            return result;

        }

        //delete student details
        public async Task DeleteStudentAsync(string enrollmentId)

        {

            if (string.IsNullOrEmpty(enrollmentId))

                throw new ArgumentException("EnrollmentId required");

            await _repo.DeleteStudentAsync(enrollmentId);

        }

        //update student details
        public async Task UpdateStudentAsync(UpdateStudentDTO dto)

        {

            if (dto == null || string.IsNullOrEmpty(dto.EnrollmentId))

                throw new ArgumentException("Invalid data");

            await _repo.UpdateStudentAsync(dto);

        }


        //generate performance for a batch
        public async Task GeneratePerformanceForBatch(string batchId)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                throw new ArgumentException("BatchId cannot be null or empty");
            }
            try
            {
               
                await _repo.GeneratePerformanceForBatch(batchId);
            }
            catch (Exception ex)
            {
                
                return;
            }
        }


        //get course dropout rate
        public async Task<double> GetCourseDropoutRateAsync(string courseId)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(courseId))
                    throw new ArgumentException("CourseId cannot be empty");
                bool exists = await _context.Course
                    .AnyAsync(c => c.CourseId == courseId);
                if (!exists)
                    throw new KeyNotFoundException($"Course {courseId} not found");
                
                var rate = await _repo.GetCourseDropoutRateAsync(courseId);
               
                if (rate < 0 || rate > 100)
                    throw new Exception("Invalid dropout rate");
                return rate;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while calculating course dropout rate", ex);
            }
        }

        //get student assessment stats
        public async Task<List<StudentAssessmentStatsDTO>> GetStudentAssessmentStatsAsync(string studentId)

        {

            if (string.IsNullOrWhiteSpace(studentId))

                throw new ArgumentException("StudentId is required");

            var studentExists = await _context.Student

                .AnyAsync(s => s.StudentId == studentId);

            if (!studentExists)

                throw new KeyNotFoundException($"Student not found: {studentId}");

            var courseIds = await _context.Enrollment

                .Where(e => e.StudentId == studentId)

                .Select(e => e.CourseId)

                .Distinct()

                .ToListAsync();

            if (!courseIds.Any())

                throw new Exception("Student not enrolled in any course");

            var result = new List<StudentAssessmentStatsDTO>();

            foreach (var courseId in courseIds)

            {

                var data = await _repo.GetStudentAssessmentStatsAsync(studentId, courseId);

                result.Add(data);

            }

            return result;

        }


    }

}
