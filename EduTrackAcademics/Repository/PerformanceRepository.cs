
using EduTrackAcademics.Data;

using EduTrackAcademics.DTO;

using EduTrackAcademics.Model;
using EduTrackAcademics.Services;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;




namespace EduTrackAcademics.Repository

{

    public class PerformanceRepository : IPerformanceRepository

    {

        private readonly EduTrackAcademicsContext _context;


        public PerformanceRepository(EduTrackAcademicsContext context)

        {

            _context = context;

        }

        // ✅ COUNT

        public async Task<int> GetPerformanceCountAsync()

        {

            return await _context.Performances.CountAsync();

        }



        private async Task<string> GenerateProgressId()

        {

            var lastId = await _context.Performances

                .OrderByDescending(p => p.ProgressID)

                .Select(p => p.ProgressID)

                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(lastId))

                return "P001";

            int num = int.Parse(lastId.Substring(1));

            return "P" + (num + 1).ToString("D3");

        }

        // ✅ ADD

        public async Task AddPerformanceAsync(Performance performance)

        {

            await _context.Performances.AddAsync(performance);

            await _context.SaveChangesAsync();

        }

        // ✅ GET LAST

        public async Task<Performance?> GetLastPerformanceAsync()

        {

            return await _context.Performances

                .OrderByDescending(p => p.LastUpdated)

                .FirstOrDefaultAsync();

        }

        // ✅ CONTENT %

        public async Task<double> GetCourseProgressPercentageAsync(string studentId, string courseId)

        {

            var moduleIds = await _context.Modules

                .Where(m => m.CourseId == courseId)

                .Select(m => m.ModuleID)

                .ToListAsync();

            if (moduleIds.Count == 0)

                return 0;

            var contentIds = await _context.Contents

                .Where(c => moduleIds.Contains(c.ModuleID))

                .Select(c => c.ContentID)

                .ToListAsync();

            int total = contentIds.Count;

            if (total == 0)

                return 0;

            int completed = await _context.StudentProgress

                .CountAsync(p =>

                    p.StudentId == studentId &&

                    contentIds.Contains(p.ContentId) &&

                    p.IsCompleted);

            double percentage = ((double)completed / total) * 100;

            return Math.Round(percentage, 2);

        }

        // ✅ LAST UPDATED (IST)

        public async Task<LastUpdatedDTO> GetLastModifiedDateAsync(string enrollmentId)

        {

            var data = await _context.Performances

                .Include(p => p.Student)

                .Include(p => p.courseBatch)

                    .ThenInclude(cb => cb.Course)

                .Include(p => p.courseBatch)

                    .ThenInclude(cb => cb.Instructor)

                .Where(p => p.EnrollmentId == enrollmentId)

                .OrderByDescending(p => p.LastUpdated)

                .FirstOrDefaultAsync();

            if (data == null)

            {

                return new LastUpdatedDTO

                {

                    EnrollmentId = enrollmentId,

                    StudentName = "No Data Found"

                };

            }

            // ✅ IST conversion

            var ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            var time = TimeZoneInfo.ConvertTimeFromUtc(data.LastUpdated, ist);

            return new LastUpdatedDTO

            {

                EnrollmentId = enrollmentId,

                StudentId = data.StudentId,

                StudentName = data.Student?.StudentName,

                CourseName = data.courseBatch?.Course?.CourseName,

                BatchId = data.BatchId,

                InstructorId = data.courseBatch?.InstructorId,

                LastUpdated = time

            };

        }


        // ✅ INSTRUCTOR BATCHES

        public async Task<List<InstructorBatchDTO>> GetInstructorBatchesAsync(string instructorId)

        {
            return await _context.CourseBatches

                .Include(cb => cb.Course)

                .Include(cb => cb.Instructor) // 🔥 IMPORTANT

                .Where(cb => cb.InstructorId == instructorId)
.Select(cb => new InstructorBatchDTO

{

    BatchId = cb.BatchId,

    CourseName = cb.Course.CourseName,

    StudentCount = _context.StudentBatchAssignments

        .Count(s => s.BatchId == cb.BatchId),

    InstructorId = cb.Instructor.InstructorId,

    InstructorName = cb.Instructor.InstructorName,

    InstructorEmail = cb.Instructor.InstructorEmail,

    InstructorPhone = cb.Instructor.InstructorPhone,

    // 🔥 NEW FIELDS

    IsActive = cb.IsActive,

    StartDate = cb.LastFilledDate,

    EndDate = cb.LastFilledDate != null

        ? cb.LastFilledDate.Value.AddDays(cb.Course.DurationInWeeks * 7)

        : (DateTime?)null
})

                .ToListAsync();

        }


        //// ✅ BATCH REPORT (FULL 🔥)



        public async Task<GetBatchReportDTO> GetBatchPerformanceAsync(string batchId)

        {

            // 🔹 Get Batch

            var batch = await _context.CourseBatches

                .Include(cb => cb.Course)

                .Include(cb => cb.Instructor)

                .FirstOrDefaultAsync(cb => cb.BatchId == batchId);

            if (batch == null)

                return null;

            // 🔹 Get Enrollments (by CourseId)

            var enrollments = await _context.Enrollment

                .Include(e => e.Student)

                .Where(e => e.CourseId == batch.CourseId)

                .ToListAsync();

            // 🔹 Remove duplicates

            var uniqueEnrollments = enrollments

                .GroupBy(e => e.EnrollmentId)

                .Select(g => g.First())

                .ToList();

            var students = new List<StudentPerformanceDTO>();

            foreach (var e in uniqueEnrollments)

            {

                // 🔹 Submissions

                var submissions = await _context.Submission

                    .Where(s => s.StudentID == e.StudentId)

                    .ToListAsync();

                var totalScore = submissions.Sum(s => s.Score);

                var lastUpdated = submissions

                    .OrderByDescending(s => s.SubmissionDate)

                    .Select(s => s.SubmissionDate)

                    .FirstOrDefault();





                // 🔥 IMPORTANT FIX → completion calculation
                var completion = await GetCourseProgressPercentageAsync(e.StudentId, batch.CourseId);





                // 🔹 Attendance

                var totalClasses = await _context.Attendances

                    .Where(a => a.BatchId == batchId && !a.IsDeleted)

                    .Select(a => a.SessionDate.Date)

                    .Distinct()

                    .CountAsync();

                var attended = await _context.Attendances

                    .Where(a => a.BatchId == batchId &&

                                a.EnrollmentID == e.EnrollmentId &&

                                a.Status == "Present" &&

                                !a.IsDeleted)

                    .CountAsync();

                double attendancePercentage = totalClasses == 0

                    ? 0

                    : (double)attended / totalClasses * 100;

                students.Add(new StudentPerformanceDTO

                {

                    StudentId = e.StudentId,

                    StudentName = e.Student.StudentName,

                    CourseName = batch.Course.CourseName,

                    AvgScore = submissions.Count == 0

                        ? 0

                        : (decimal)submissions.Average(s => s.Score),

                    CompletionPercentage = completion, // ✅ FIXED

                    AttendancePercentage = attendancePercentage,

                    TotalScore = totalScore,

                    EnrollmentId = e.EnrollmentId,

                    LastUpdated = lastUpdated

                });

            }

            // 🔹 Batch Calculations

            decimal batchAvgScore = students.Count == 0

                ? 0

                : students.Average(s => s.AvgScore);

            double batchAvgAttendance = students.Count == 0

                ? 0

                : students.Average(s => s.AttendancePercentage);

            double batchAvgCompletion = students.Count == 0

                ? 0

                : students.Average(s => s.CompletionPercentage);

            var topPerformer = students

                .OrderByDescending(s => s.AvgScore)

                .FirstOrDefault();

            int completedStudents = students.Count(s => s.CompletionPercentage == 100);

            var batchLastUpdated = students

                .Where(s => s.LastUpdated != null)

                .OrderByDescending(s => s.LastUpdated)

                .Select(s => s.LastUpdated)

                .FirstOrDefault();

            // 🔹 Final DTO

            return new GetBatchReportDTO

            {

                BatchId = batchId,

                CourseName = batch.Course.CourseName,
                CourseId= batch.CourseId,

                InstructorId = batch.InstructorId,

                InstructorName = batch.Instructor.InstructorName,

                Students = students,

                TotalStudents = students.Count,

                BatchAverageScore = Math.Round(batchAvgScore, 2),

                BatchAverageAttendance = Math.Round((decimal)batchAvgAttendance, 2),

                BatchAverageCompletionPercentage = Math.Round((decimal)batchAvgCompletion, 2),

                TopPerformer = topPerformer?.StudentName,

                CompletedStudents = completedStudents,

                LastUpdated = batchLastUpdated

            };

        }


        public async Task<List<BatchCompletionDTO>> GetBatchCompletionByInstructor(string instructorId)

        {

            var result = new List<BatchCompletionDTO>();

            var batches = await _context.CourseBatches

                .Where(b => b.InstructorId == instructorId)

                .ToListAsync();

            foreach (var batch in batches)

            {

                var students = await _context.StudentBatchAssignments

                    .Where(s => s.BatchId == batch.BatchId)

                    .Select(s => s.StudentId)

                    .ToListAsync();

                double total = 0;

                int count = 0;

                foreach (var studentId in students)

                {

                    var progress = await GetCourseProgressPercentageAsync(studentId, batch.CourseId);

                    total += progress;

                    count++;

                }

                double completion = count == 0 ? 0 : Math.Round(total / count, 2);

                result.Add(new BatchCompletionDTO

                {

                    BatchId = batch.BatchId,

                    Completion = completion

                });

            }

            return result;

        }


        public async Task<List<CourseBatch>> GetBatchesByInstructor(string instructorId)
        {
            return await _context.CourseBatches
                .Where(cb => cb.InstructorId == instructorId)
                .ToListAsync();

        }

        public async Task<List<InstructorBatchDTO>> GetAllBatchesAsync()

        {

            return await _context.CourseBatches

                .Include(cb => cb.Course)

                .Include(cb => cb.Instructor)

                .Select(cb => new InstructorBatchDTO

                {

                    BatchId = cb.BatchId,

                    CourseName = cb.Course.CourseName,

                    StudentCount = _context.StudentBatchAssignments

                        .Count(s => s.BatchId == cb.BatchId),

                    InstructorId = cb.Instructor.InstructorId,

                    InstructorName = cb.Instructor.InstructorName,

                    InstructorEmail = cb.Instructor.InstructorEmail,

                    InstructorPhone = cb.Instructor.InstructorPhone,

                    IsActive = cb.IsActive,

                    StartDate = cb.LastFilledDate,

                    EndDate = cb.LastFilledDate != null

                        ? cb.LastFilledDate.Value.AddDays(cb.Course.DurationInWeeks * 7)

                        : (DateTime?)null

                })

                .ToListAsync();

        }
        public async Task<List<BatchClassCountDTO>> GetBatchClassCountsByInstructor(string instructorId)

        {

            var result = new List<BatchClassCountDTO>();

            // Step 1: Get all batches of instructor

            var batches = await _context.CourseBatches

                .Where(b => b.InstructorId == instructorId)

                .ToListAsync();

            // Step 2: Loop each batch

            foreach (var batch in batches)

            {

                // Step 3: Count total classes (distinct dates)

                var totalClasses = await _context.Attendances

                    .Where(a => a.BatchId == batch.BatchId && !a.IsDeleted)

                    .Select(a => a.SessionDate.Date)

                    .Distinct()

                    .CountAsync();

                // Step 4: Add to result

                result.Add(new BatchClassCountDTO

                {

                    BatchId = batch.BatchId,

                    TotalClasses = totalClasses

                });

            }

            return result;

        }
        public async Task<List<BatchStartDateDTO>> GetBatchStartDatesAsync()

        {

            return await _context.CourseBatches

                .Where(b => b.LastFilledDate != null)

                .Select(b => new BatchStartDateDTO

                {

                    BatchId = b.BatchId,

                    StartDate = b.LastFilledDate,
                    EndDate=b.LastFilledDate.Value.AddDays(b.Course.DurationInWeeks * 7)

                })

                .OrderBy(b => b.StartDate) // sorting here itself

                .ToListAsync();

        }
        public async Task DeleteStudentAsync(string enrollmentId)

        {

            var enrollment = await _context.Enrollment

                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

            if (enrollment == null)

                throw new Exception("Student not found");

            _context.Enrollment.Remove(enrollment);

            await _context.SaveChangesAsync();

        }
        public async Task UpdateStudentAsync(UpdateStudentDTO dto)

        {

            var enrollment = await _context.Enrollment

                .Include(e => e.Student)

                .FirstOrDefaultAsync(e => e.EnrollmentId == dto.EnrollmentId);

            if (enrollment == null)

                throw new Exception("Student not found");

            // Update student name

            enrollment.Student.StudentName = dto.StudentName;

            // Optional (if course editable)

            enrollment.CourseId = dto.CourseId;

            await _context.SaveChangesAsync();

        }




        //------------------------------
        public async Task GeneratePerformanceForBatch(string batchId)

        {

            // ✅ Get Batch

            var batch = await _context.CourseBatches

                .FirstOrDefaultAsync(b => b.BatchId == batchId);

            if (batch == null)

                throw new Exception("Batch not found");

            // ✅ Get Enrollments (Course based)

            var enrollments = await _context.Enrollment

                .Include(e => e.Student)

                .Include(e => e.Course)

                .Where(e => e.CourseId == batch.CourseId)

                .ToListAsync();

            if (!enrollments.Any())

                throw new Exception("No enrollments found");

            foreach (var enrollment in enrollments)

            {

                // ✅ Get submissions

                var submissions = await _context.Submission

                    .Where(s => s.StudentID == enrollment.StudentId)

                    .ToListAsync();

                // ✅ Avg Score

                decimal avgScore = submissions.Any()

                    ? submissions.Average(s => (decimal)s.Score)

                    : 0;

                // ✅ Completion %

                double completion = await GetCourseProgressPercentageAsync(

                    enrollment.StudentId,

                    enrollment.CourseId);

                decimal finalCompletion = Math.Round((decimal)completion, 2);

                // ✅ Check existing

                var existing = await _context.Performances

                    .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.EnrollmentId);

                if (existing != null)

                {

                    existing.AvgScore = avgScore;

                    existing.CompletionPercentage = finalCompletion;

                    existing.LastUpdated = DateTime.UtcNow;

                }

                else

                {

                    // 🔥 Simple safe ID generation

                    var count = await _context.Performances.CountAsync();

                    string progressId = "P" + (count + 1).ToString("D3");

                    await _context.Performances.AddAsync(new Performance

                    {

                        ProgressID = progressId,

                        EnrollmentId = enrollment.EnrollmentId,

                        StudentId = enrollment.StudentId,

                        AvgScore = avgScore,

                        CompletionPercentage = finalCompletion,

                        LastUpdated = DateTime.UtcNow,

                        BatchId = batch.BatchId,

                        InstructorId = batch.InstructorId ?? "I001"

                    });

                }

                // 🔥 IMPORTANT (SAVE EACH TIME)

                await _context.SaveChangesAsync();

            }

        }


        public async Task<double> GetCourseDropoutRateAsync(string courseId)
        {
            var enrollments = await _context.Enrollment
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
            int totalStudents = enrollments
                .Select(e => e.StudentId)
                .Distinct()
                .Count();
            int droppedStudents = enrollments
                .Where(e => e.Status == "Dropped")
                .Select(e => e.StudentId)
                .Distinct()
                .Count();
            if (totalStudents == 0)
                return 0;
            return (double)droppedStudents / totalStudents * 100;
        }
        public async Task<StudentAssessmentStatsDTO> GetStudentAssessmentStatsAsync(string studentId, string courseId)

        {

            // Total assessments in that course

            var total = await _context.Assessments

                .Where(a => a.CourseId == courseId)

                .CountAsync();

            // Submitted assessments by student for that course

            var submitted = await (

                from s in _context.Submission

                join a in _context.Assessments

                on s.AssessmentId equals a.AssessmentID

                where s.StudentID == studentId && a.CourseId == courseId

                select s.AssessmentId

            ).Distinct().CountAsync();

            return new StudentAssessmentStatsDTO

            {

                CourseId = courseId,   // 🔥 IMPORTANT

                TotalAssessments = total,

                SubmittedAssessments = submitted,

                PendingAssessments = total - submitted

            };

        }










    }
}
