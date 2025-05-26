using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var tenantId = context.User.FindFirst("TenantId")?.Value;
                if (string.IsNullOrEmpty(tenantId))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(
                        new ApiResponse<object>
                        {
                            Error = new ErrorResponse
                            {
                                Code = "Forbidden",
                                Message = "TenantId is missing."
                            }
                        }
                    );
                    return;
                }
            }
            await _next(context);
        }
    }
}
