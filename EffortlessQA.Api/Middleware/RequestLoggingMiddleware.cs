using EffortlessQA.Data.Dtos;
using Serilog;

namespace EffortlessQA.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Log.Information(
                "Request {Method} {Path} started",
                context.Request.Method,
                context.Request.Path
            );

            try
            {
                await _next(context);
                Log.Information(
                    "Request {Method} {Path} completed with status {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode
                );
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Error in request {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path
                );
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(
                    new ApiResponse<object>
                    {
                        Error = new ErrorResponse
                        {
                            Code = "InternalServerError",
                            Message = ex.Message
                        }
                    }
                );
            }
        }
    }
}
