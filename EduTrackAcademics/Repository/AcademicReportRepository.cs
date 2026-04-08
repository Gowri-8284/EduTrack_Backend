using System.Collections.Generic;
using System.Threading.Tasks;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Dummy;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;


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


		var existing = await _context.AcademicReport


			.FirstOrDefaultAsync(r => r.Course == batch.CourseName);


		if (existing != null)


		{


			// UPDATE
		existing.AvgScore = batch.BatchAverageScore;


			existing.CompletionRate = batch.BatchAverageCompletionPercentage;


			existing.BatchAverageAttendance = batch.BatchAverageAttendance;


			existing.StudentAttendance = batch.StudentAverageAttendence;


			existing.GeneratedDate = DateTime.Now;


		}


		else


		{


			// INSERT
		int count = await _context.AcademicReport.CountAsync();

			var Report_Id = $"R{(count + 1):D3}";

			var report = new AcademicReport


			{


				ReportId = Report_Id,


				Course = batch.CourseName,


				AvgScore = batch.BatchAverageScore,


				CompletionRate = batch.BatchAverageCompletionPercentage,


				BatchAverageAttendance = batch.BatchAverageAttendance,


				StudentAttendance = batch.StudentAverageAttendence,


				DropOutRate = 0,


				GeneratedDate = DateTime.Now


			};


			_context.AcademicReport.Add(report);


		}


	}


	await _context.SaveChangesAsync();


}


        }


    }