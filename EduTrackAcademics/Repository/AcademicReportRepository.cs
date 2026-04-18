using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Dummy;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace EduTrackAcademics.Repository

{
    public class AcademicReportRepository : IAcademicReportRepository

    {

        private readonly EduTrackAcademicsContext _context;

        private readonly IPerformanceRepository _performanceRepository;

        public AcademicReportRepository(

            EduTrackAcademicsContext context,

            IPerformanceRepository performanceRepository)

        {

            _context = context;

            _performanceRepository = performanceRepository;

        }

        // ✅ SINGLE BATCH

        public async Task<GetBatchReportDTO> GetBatchReport(string batchId)

        {

            var batch = await _context.CourseBatches

                .Include(b => b.Course)

                .FirstOrDefaultAsync(b => b.BatchId == batchId);

            if (batch == null)

                return null;

            var perf = await _performanceRepository

                .GetBatchPerformanceAsync(batchId);

            if (perf == null)

                return null;

            var instructor = await _context.Instructor

                .FirstOrDefaultAsync(i => i.InstructorId == batch.InstructorId);

            int completed = perf.Students

                .Count(s => s.CompletionPercentage == 100);

            var top = perf.Students

                .OrderByDescending(s => s.AvgScore)

                .FirstOrDefault();

            var lastUpdated = perf.Students.Any()

                ? perf.Students.Max(s => s.LastUpdated)

                : DateTime.Now;

            return new GetBatchReportDTO

            {

                BatchId = batch.BatchId,

                CourseName = batch.Course.CourseName,

                BatchAverageCompletionPercentage = perf.BatchAverageCompletionPercentage,

                BatchAverageAttendance = perf.BatchAverageAttendance,

                StudentAverageAttendence = perf.StudentAverageAttendence,

                Students = perf.Students,

                TotalStudents = perf.TotalStudents,

                BatchAverageScore = perf.BatchAverageScore,

                TopPerformer = top?.StudentName,

                CompletedStudents = completed,

                LastUpdated = lastUpdated,

                InstructorId = instructor?.InstructorId,

                InstructorName = instructor?.InstructorName,


            };

        }

        // ✅ ALL BATCHES

        public async Task<AcademicReportDTO> GetFullAcademicReport()

        {

            var batches = await _context.CourseBatches

                .Include(b => b.Course)

                .ToListAsync();

            var result = new List<GetBatchReportDTO>();

            foreach (var batch in batches)

            {

                var perf = await _performanceRepository

                    .GetBatchPerformanceAsync(batch.BatchId);

                if (perf == null)

                    continue;

                var instructor = await _context.Instructor

                    .FirstOrDefaultAsync(i => i.InstructorId == batch.InstructorId);

                int completed = perf.Students

                    .Count(s => s.CompletionPercentage == 100);

                var top = perf.Students

                    .OrderByDescending(s => s.AvgScore)

                    .FirstOrDefault();

                var lastUpdated = perf.Students.Any()

                    ? perf.Students.Max(s => s.LastUpdated)

                    : DateTime.Now;

                result.Add(new GetBatchReportDTO

                {

                    BatchId = batch.BatchId,

                    CourseName = batch.Course.CourseName,

                    BatchAverageCompletionPercentage = perf.BatchAverageCompletionPercentage,

                    BatchAverageAttendance = perf.BatchAverageAttendance,

                    StudentAverageAttendence = perf.StudentAverageAttendence,

                    Students = perf.Students,

                    TotalStudents = perf.TotalStudents,

                    BatchAverageScore = perf.BatchAverageScore,

                    TopPerformer = top?.StudentName,

                    CompletedStudents = completed,

                    LastUpdated = lastUpdated,

                    InstructorId = instructor?.InstructorId,

                    InstructorName = instructor?.InstructorName,


                });

            }

            return new AcademicReportDTO

            {

                Batches = result

            };

        }

        // 💾 SAVE / UPDATE

        public async Task SaveOrUpdateAcademicReport(AcademicReportDTO dto)
        {
            foreach (var batch in dto.Batches)
            {
                var perf = await _performanceRepository.GetBatchPerformanceAsync(batch.BatchId);
                if (perf == null || perf.Students == null) continue;

                // Use a unique seed or Guid to ensure no collisions during the loop
                int i = 1;
                foreach (var student in perf.Students)
                {
                    var sid = !string.IsNullOrEmpty(student.StudentId) ? student.StudentId : Guid.NewGuid().ToString("N").Substring(0, 5);

                    // RECOMMENDATION: Use a more robust ID format or let the DB handle it
                    var reportId = $"R_{batch.BatchId}_{sid}";

                    // Check if the entity is ALREADY being tracked in this session to avoid the exception
                    var existing = _context.AcademicReport.Local.FirstOrDefault(r => r.ReportId == reportId)
                                   ?? await _context.AcademicReport.FirstOrDefaultAsync(r => r.ReportId == reportId);

                    if (existing != null)
                    {
                        // UPDATE existing instance
                        existing.AvgScore = student.AvgScore;
                        existing.CompletionRate = (decimal)student.CompletionPercentage;
                        existing.StudentAttendance = (decimal)student.AttendancePercentage;
                        existing.BatchAverageAttendance = batch.BatchAverageAttendance;
                        existing.GeneratedDate = DateTime.Now;
                    }
                    else
                    {
                        // INSERT new instance
                        var report = new AcademicReport
                        {
                            ReportId = reportId,
                            Course = batch.CourseName,
                            AvgScore = student.AvgScore,
                            CompletionRate = (decimal)student.CompletionPercentage,
                            BatchAverageAttendance = batch.BatchAverageAttendance,
                            StudentAttendance = (decimal)student.AttendancePercentage,
                            DropOutRate = 0,
                            GeneratedDate = DateTime.Now
                        };
                        await _context.AcademicReport.AddAsync(report);
                    }
                    i++;
                }
            }
            await _context.SaveChangesAsync();
        }




    }
}