using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MyApi.Nuova_cartella
{
    public class GlobalExcepetionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExcepetionMiddleware> logger;

        public GlobalExcepetionMiddleware(RequestDelegate next, ILogger<GlobalExcepetionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await next(context); }
            catch (Exception ex)
            {
                logger.LogError("--[ECCEZIONE NON GESTITA]--");
                await HandlerException(context, ex);
                throw;
            }
        }
        private static Task HandlerException(HttpContext context, Exception ex)
        {
            var (statusCode, title) = ex switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                InvalidProgramException => (StatusCodes.Status406NotAcceptable, "Invalid Operation"),
                ValidationException => (StatusCodes.Status400BadRequest, "Bad Request"),
                _ => (StatusCodes.Status500InternalServerError, "Server Error - Non è colpa tua tranquillo")
            };
            var traceid = Activity.Current?.Id ?? context.TraceIdentifier;
            var problemDetails = new ProblemDetails()
            {
                Detail = ex.Message,
                Status = statusCode,
                Title = title,
                Instance = context.Request.Path
            };
            problemDetails.Extensions["TraceID"] = traceid;
            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
