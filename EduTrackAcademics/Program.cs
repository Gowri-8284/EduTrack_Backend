using System.Text;
using EduTrackAcademics.Aspects;

using EduTrackAcademics.AuthFolder;

using EduTrackAcademics.Data;
using EduTrackAcademics.Dummy;

using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using EduTrackAcademics.Service;
using EduTrackAcademics.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;





var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<EduTrackAcademicsContext>(options =>
options.UseSqlServer(

builder.Configuration.GetConnectionString("EduTrackAcademicsContext")

?? throw new InvalidOperationException("Connection string not found")

));


builder.Services.AddControllers(); builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<ICoordinatorDashboardRepo, CoordinatorDashboardRepo>();
builder.Services.AddScoped<ICoordinatorDashboardService, CoordinatorDashboardService>();
builder.Services.AddSingleton<DummyInstructorData>();

builder.Services.AddSingleton<DummyInstructor>(); 
builder.Services.AddSingleton<DummyStudent>(); 
builder.Services.AddSingleton<DummyInstructorReg>();

builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>(); 
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

builder.Services.AddScoped<IdService>(); 
builder.Services.AddScoped<PasswordService>();
//builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IPerformanceRepository, PerformanceRepository>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>(); 
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

//builder.Services.AddScoped<IStudentProgressesRepository, StudentProgressesRepository>();//builder.Services.AddScoped<IStudentProgressesService, StudentProgressesService>();// Student Profile
builder.Services.AddScoped<IStudentProfileService, StudentProfileService>();
builder.Services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();


// Auth
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<JWTTokenGenerator>();

//for authentication
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddScoped<IInstructorAssessmentService, InstructorAssessmentService>();
builder.Services.AddScoped<IInstructorAssessmentRepository, InstructorAssessmentRepository>();
builder.Services.AddScoped<IInstructorModuleService, InstructorModuleService>();
builder.Services.AddScoped<IInstructorModuleRepository, InstructorModuleRepository>();
builder.Services.AddScoped<IInstructorAttendanceService, InstructorAttendanceService>();
builder.Services.AddScoped<IInstructorAttendanceRepository, InstructorAttendanceRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();



// =======================// JWT Authentication// =======================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

  .AddJwtBearer(options =>
   {

	   options.TokenValidationParameters = new TokenValidationParameters
	   {

		   ValidateIssuer = true,

		   ValidateAudience = true,

		   ValidateLifetime = true,

		   ValidateIssuerSigningKey = true,

		   IssuerSigningKey = new SymmetricSecurityKey(

	   Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])

	   ),

		   ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

		   ValidAudience = builder.Configuration["JwtSettings:Audience"],
		   RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"

	   };

   });

builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>(); 
builder.Services.AddScoped<ISubmissionService, SubmissionService>();

builder.Services.AddScoped<IAcademicReportRepository, AcademicReportRepository>();
builder.Services.AddScoped<IAcademicReportService, AcademicReportService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<CoordinatorDashboardService>();
builder.Services.AddScoped<CoordinatorJobs>();
builder.Services.AddScoped<IDeadlineReminderService, DeadlineReminderService>();
builder.Services.AddScoped<BatchCleanupService>();


builder.Services.AddCors(options =>
{

	options.AddPolicy("MyCorsPolicy",

	policy =>
	{

		policy.WithOrigins("http://localhost:5173")

	  .AllowAnyHeader()

	  .AllowAnyMethod();


	});


});

builder.Services.AddHangfire(config =>
{

config.UseMemoryStorage(); // or UseSqlServerStorage if using database
});

builder.Services.AddHangfire(config =>
config.UseSqlServerStorage(builder.Configuration.GetConnectionString("EduTrackAcademicsContext"))

); builder.Services.AddHangfireServer();


var app = builder.Build(); 


using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<EduTrackAcademicsContext>();
	var admin = context.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");

	if (admin != null)
	{
		if (!admin.Password.StartsWith("$2"))
		{
			admin.Password = BCrypt.Net.BCrypt.HashPassword("Admin@123");
			context.SaveChanges();
		}
	}
}
using (var scope = app.Services.CreateScope())
{
	var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

	// This schedules the job to run every morning at 8:00 AM
	recurringJobManager.AddOrUpdate<IDeadlineReminderService>(
		"daily-assessment-deadline-check", // Unique ID for the job
		service => service.CheckAndSendDeadlineRemindersAsync(),
		Cron.Daily(8)
	);
	recurringJobManager.AddOrUpdate<BatchCleanupService>(
		"auto-delete-expired-batches",
		service => service.ProcessExpiredBatches(),
		Cron.Daily // Runs at 00:00
	);
}

// =======================// Middleware// =======================

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSwagger();app.UseSwaggerUI(c =>
{

	c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduTrack API v1");

	c.RoutePrefix = "swagger";

});

app.UseHttpsRedirection(); 
app.UseStaticFiles();

app.UseCors("MyCorsPolicy");

app.UseAuthentication(); 
app.UseAuthorization();


app.MapControllers(); 
app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<CoordinatorJobs>("auto-assign-pending-students", job => job.AutoAssignPendingStudentsJob(),


Cron.Minutely);

app.Run();

