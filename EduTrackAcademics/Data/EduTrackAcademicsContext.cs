using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EduTrackAcademics.Model;
using EduTrackAcademics.Services;
using Microsoft.EntityFrameworkCore;
namespace EduTrackAcademics.Data

{

	public class EduTrackAcademicsContext : DbContext
	{

		public EduTrackAcademicsContext(DbContextOptions<EduTrackAcademicsContext> options)

		: base(options)

		{

		}


		public DbSet<EduTrackAcademics.Model.Users> Users { get; set; } = default!;

		public DbSet<EduTrackAcademics.Model.Course> Course { get; set; } = default!;
		public DbSet<EduTrackAcademics.Model.Student> Student { get; set; }
		public DbSet<EduTrackAcademics.Model.StudentAdditionalDetails> StudentAdditionalDetails { get; set; }
		public DbSet<EduTrackAcademics.Model.Submission> Submission { get; set; }
		//public DbSet<EduTrackAcademics.Model.StudentLoginHistory> AuditLog { get; set; }
		public DbSet<EduTrackAcademics.Model.Instructor> Instructor { get; set; }

		public DbSet<EduTrackAcademics.Model.Coordinator> Coordinator { get; set; }

		public DbSet<EduTrackAcademics.Model.Qualification> Qualification { get; set; }

		public DbSet<EduTrackAcademics.Model.AcademicYear> AcademicYear { get; set; }

		public DbSet<EduTrackAcademics.Model.CourseAssignment> CourseAssignment { get; set; }

		public DbSet<EduTrackAcademics.Model.ProgramEntity> Programs { get; set; }

		public DbSet<EduTrackAcademics.Model.Question> Questions { get; set; }

		public DbSet<EduTrackAcademics.Model.Assessment> Assessments { get; set; }

		public DbSet<EduTrackAcademics.Model.Attendance> Attendances { get; set; }

		public DbSet<EduTrackAcademics.Model.Module> Modules { get; set; }

		public DbSet<EduTrackAcademics.Model.Content> Contents { get; set; }

		public DbSet<StudentCourseAssignment> StudentCourseAssignments { get; set; }

		public DbSet<InstructorCourseAssignment> InstructorCourseAssignments { get; set; }

		public DbSet<CourseBatch> CourseBatches { get; set; }

		public DbSet<Notification> Notification { get; set; }

		public DbSet<StudentBatchAssignment> StudentBatchAssignments { get; set; }


		public DbSet<Performance> Performances { get; set; }
		public DbSet<NotificationUserStatus> NotificationUserStatus { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<StudentBatchAssignment>()
		.HasKey(x => new { x.StudentId, x.BatchId });



			modelBuilder.Entity<Attendance>()
				.HasOne(a => a.StudentBatchAssignment)
				.WithMany()
				.HasForeignKey(a => new { a.StudentId, a.BatchId });

			modelBuilder.Entity<Qualification>()

			.HasMany(q => q.Programs)

			.WithOne(p => p.Qualification)

			.HasForeignKey(p => p.QualificationId);

			modelBuilder.Entity<Assessment>()
	.HasOne(a => a.Course)
	.WithMany(c => c.Assessments)
	.HasForeignKey(a => a.CourseId);
			modelBuilder.Entity<StudentBatchAssignment>()
.HasKey(sba => new { sba.BatchId, sba.StudentId });

modelBuilder.Entity<StudentBatchAssignment>()
	.HasOne(sba => sba.CourseBatch)
	.WithMany(cb => cb.StudentBatchAssignments)
	.HasForeignKey(sba => sba.BatchId);
			modelBuilder.Entity<StudentBatchAssignment>()
				.HasOne(sba => sba.Student)
				.WithMany();


			modelBuilder.Entity<ProgramEntity>()

			.HasMany(p => p.AcademicYears)

			.WithOne(y => y.Program)

			.HasForeignKey(y => y.ProgramId);


			modelBuilder.Entity<AcademicYear>()

			.HasMany(y => y.Courses)

			.WithOne(c => c.AcademicYear)

			.HasForeignKey(c => c.AcademicYearId);


			modelBuilder.Entity<Users>()

			.HasOne(u => u.Student)

			.WithOne(s => s.User)

			.HasForeignKey<Student>(s => s.UserId);


			modelBuilder.Entity<Users>()

			.HasOne(u => u.Instructor)

			.WithOne(i => i.User)

			.HasForeignKey<Instructor>(i => i.UserId);


			modelBuilder.Entity<Users>()

			.HasOne(u => u.Coordinator)

			.WithOne(c => c.User)

			.HasForeignKey<Coordinator>(c => c.UserId);


			modelBuilder.Entity<Attendance>()
				.HasOne(a => a.Enrollment)
				.WithMany(e => e.Attendances)
				.HasForeignKey(a => a.EnrollmentID)
				.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<StudentBatchAssignment>()
	.HasKey(sba => new { sba.StudentId, sba.BatchId });

		
			modelBuilder.Entity<Attendance>()
				.HasOne(a => a.StudentBatchAssignment)
				.WithMany(sba => sba.Attendances)
				.HasForeignKey(a => new { a.StudentId, a.BatchId })
				.OnDelete(DeleteBehavior.Restrict);


			modelBuilder.Entity<Assessment>()
				.HasOne(a => a.Course)
				.WithMany(c => c.Assessments)
				.HasForeignKey(a => a.CourseId);
			modelBuilder.Entity<BatchConfig>()
.HasKey(b => b.CourseId);

			modelBuilder.Entity<BatchConfig>()

			.HasKey(b => b.CourseId);


			modelBuilder.Entity<BatchConfig>()

			.HasKey(b => b.CourseId);


			string adminHashedPassword = "$2a$12$zQYc9Zkz5PxyDZ0GmJ3q6Oc0Q4r1Uju9rG0q3JcPlzLzF8qfGZpMe";
	
			modelBuilder.Entity<Users>().HasData(new Users			{
			UserId = 1, 		
						Email = "admin@gmail.com",
				Password = adminHashedPassword,

				Role = "Admin",
				IsEmailVerified = true          });

		}


		public DbSet<BatchConfig> BatchConfigs { get; set; }
		public DbSet<EduTrackAcademics.Model.Enrollment> Enrollment { get; set; } = default!;
		public DbSet<EduTrackAcademics.Model.StudentProgress> StudentProgress { get; set; } = default!;


		public DbSet<EduTrackAcademics.Model.StudentAnswer> StudentAnswer { get; set; } = default!;

		public DbSet<EduTrackAcademics.Model.AcademicReport> AcademicReport { get; set; }

		public DbSet<AcademicRule> AcademicRules { get; set; }

	}
}


