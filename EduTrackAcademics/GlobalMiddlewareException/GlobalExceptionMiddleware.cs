using System.Net;
using System.Text.Json;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;      // Points to your specific exceptions
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EduTrackAcademics.Aspects
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception caught by middleware.");
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";

			// Default values
			var statusCode = (int)HttpStatusCode.InternalServerError;
			var message = "An unexpected server error occurred.";

			// Pattern Matching for your specific folder hierarchy
			switch (exception)
			{
				// 1. Catch anything inheriting from your /Base/ folder
				case BaseAppException baseEx:
					statusCode = baseEx.StatusCode;
					message = baseEx.Message;
					break;

				// 2. Catch standard System exceptions
				case UnauthorizedAccessException:
					statusCode = (int)HttpStatusCode.Unauthorized;
					message = "Access denied. Please login.";
					break;

				case ArgumentException:
					statusCode = (int)HttpStatusCode.BadRequest;
					message = exception.Message;
					break;
			}

			context.Response.StatusCode = statusCode;

			var response = new ErrorResponse
			{
				StatusCode = statusCode,
				Message = message,
				// Helpful for debugging in your 'frontend' dev console
				//Details = _env.IsDevelopment() ? exception.Message : null
			};

			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
		}
	}
}