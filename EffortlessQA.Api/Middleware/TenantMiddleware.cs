using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Api.Middleware
{
    //public class TenantMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    public TenantMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        if (context.User.Identity.IsAuthenticated)
    //        {
    //            var tenantId = context.User.FindFirst("TenantId")?.Value;
    //            if (string.IsNullOrEmpty(tenantId))
    //            {
    //                context.Response.StatusCode = StatusCodes.Status403Forbidden;
    //                await context.Response.WriteAsJsonAsync(
    //                    new ApiResponse<object>
    //                    {
    //                        Error = new ErrorResponse
    //                        {
    //                            Code = "Forbidden",
    //                            Message = "TenantId is missing."
    //                        }
    //                    }
    //                );
    //                return;
    //            }
    //        }
    //        await _next(context);
    //    }
    //}
    public class TenantValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, EffortlessQAContext dbContext)
        {
            var endpoint = context.Request.Path.Value;
            if (
                endpoint != null
                && endpoint.Contains("/api/auth/login", StringComparison.OrdinalIgnoreCase)
            )
            {
                await _next(context); // Skip validation for login
                return;
            }

            var cookieTenantId = context.Request.Cookies["TenantId"];
            var jwtTenantId = context.User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(cookieTenantId) || string.IsNullOrEmpty(jwtTenantId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(
                    new ApiResponse<object>
                    {
                        Error = new ErrorResponse
                        {
                            Code = "TenantIdMissing",
                            Message = "TenantId is missing in cookie or JWT."
                        }
                    }
                );
                return;
            }

            if (cookieTenantId != jwtTenantId)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(
                    new ApiResponse<object>
                    {
                        Error = new ErrorResponse
                        {
                            Code = "TenantIdMismatch",
                            Message = "TenantId in cookie does not match JWT."
                        }
                    }
                );
                return;
            }

            // Verify TenantId exists in the database
            var tenantExists = await dbContext.Tenants.AnyAsync(t =>
                t.Id == cookieTenantId && !t.IsDeleted
            );
            if (!tenantExists)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(
                    new ApiResponse<object>
                    {
                        Error = new ErrorResponse
                        {
                            Code = "InvalidTenant",
                            Message = "The specified TenantId does not exist."
                        }
                    }
                );
                return;
            }

            await _next(context);
        }
    }
}
