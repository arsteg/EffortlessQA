using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            // POST /api/v1/auth/register
            app.MapPost(
                    "/api/v1/auth/register",
                    async ([FromBody] RegisterDto dto, IAuthService authService) =>
                    {
                        try
                        {
                            var result = await authService.RegisterAsync(dto);
                            return Results.Ok(
                                new ApiResponse<UserDto>
                                {
                                    Data = result,
                                    Meta = new { Message = "User registered successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("Register")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/auth/tenant/current
            app.MapGet(
                    "api/v1/Auth/tenantCurrent",
                    async (IAuthService authService) =>
                    {
                        try
                        {
                            var result = await authService.GetCurrentTenantAsync();
                            return Results.Ok(
                                new ApiResponse<TenantDto>
                                {
                                    Data = result,
                                    Meta = new { Message = "User registered successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("tenantCurrent")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/auth/login
            app.MapPost(
                    "/api/v1/auth/login",
                    async (
                        [FromBody] LoginDto dto,
                        IAuthService authService,
                        HttpContext context
                    ) =>
                    {
                        if (
                            dto == null
                            || string.IsNullOrWhiteSpace(dto.Email)
                            || string.IsNullOrWhiteSpace(dto.Password)
                        )
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "InvalidInput",
                                        Message = "Email and password are required."
                                    }
                                }
                            );
                        }

                        try
                        {
                            var token = await authService.LoginAsync(dto);
                            return Results.Ok(
                                new ApiResponse<string>
                                {
                                    Data = token,
                                    Meta = new { Message = "Login successful" }
                                }
                            );
                        }
                        catch (UnauthorizedAccessException)
                        {
                            return Results.Unauthorized();
                        }
                        catch (Exception ex)
                        {
                            return Results.StatusCode(500);
                        }
                    }
                )
                .WithName("Login")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/auth/logout
            app.MapPost(
                    "/api/v1/auth/logout",
                    (HttpContext context) =>
                    {
                        context.Response.Cookies.Delete(
                            "access_token",
                            new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            }
                        );

                        context.Response.Cookies.Delete(
                            "TenantId",
                            new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            }
                        );

                        return Results.Ok(
                            new ApiResponse<object>
                            {
                                Data = null,
                                Meta = new { Message = "Logout successful" }
                            }
                        );
                    }
                )
                .WithName("Logout")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/auth/oauth/google
            app.MapPost(
                    "/api/v1/auth/oauth/google",
                    async ([FromBody] OAuthLoginDto dto, IAuthService authService) =>
                    {
                        try
                        {
                            var token = await authService.OAuthLoginAsync(dto, "Google");
                            return Results.Ok(
                                new ApiResponse<string>
                                {
                                    Data = token,
                                    Meta = new { Message = "Google OAuth login successful" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("GoogleOAuthLogin")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/auth/oauth/github
            app.MapPost(
                    "/api/v1/auth/oauth/github",
                    async ([FromBody] OAuthLoginDto dto, IAuthService authService) =>
                    {
                        try
                        {
                            var token = await authService.OAuthLoginAsync(dto, "GitHub");
                            return Results.Ok(
                                new ApiResponse<string>
                                {
                                    Data = token,
                                    Meta = new { Message = "GitHub OAuth login successful" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("GitHubOAuthLogin")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // GET /api/v1/users/me
            app.MapGet(
                    "/api/v1/users/me",
                    async (HttpContext context, IAuthService authService) =>
                    {
                        try
                        {
                            var userId = context.User.FindFirst("sub")?.Value;
                            if (string.IsNullOrEmpty(userId))
                            {
                                return Results.Unauthorized();
                            }
                            var user = await authService.GetUserProfileAsync(Guid.Parse(userId));
                            return Results.Ok(
                                new ApiResponse<UserDto>
                                {
                                    Data = user,
                                    Meta = new { Message = "User profile retrieved successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("GetUserProfile")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // PUT /api/v1/users/me
            app.MapPut(
                    "/api/v1/users/me",
                    async (
                        [FromBody] UpdateUserDto dto,
                        HttpContext context,
                        IAuthService authService
                    ) =>
                    {
                        try
                        {
                            var userId = context.User.FindFirst("sub")?.Value;
                            if (string.IsNullOrEmpty(userId))
                            {
                                return Results.Unauthorized();
                            }
                            var user = await authService.UpdateUserProfileAsync(
                                Guid.Parse(userId),
                                dto
                            );
                            return Results.Ok(
                                new ApiResponse<UserDto>
                                {
                                    Data = user,
                                    Meta = new { Message = "User profile updated successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("UpdateUserProfile")
                .RequireAuthorization("TesterOrAdmin")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/users/me/password-reset
            app.MapPost(
                    "/api/v1/users/me/password-reset",
                    async (
                        [FromBody] PasswordResetRequestDto dto,
                        IAuthService authService,
                        IEmailService emailService
                    ) =>
                    {
                        try
                        {
                            await authService.RequestPasswordResetAsync(dto, emailService);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Password reset email sent" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("RequestPasswordReset")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/users/me/password-reset/confirm
            app.MapPost(
                    "/api/v1/users/me/password-reset/confirm",
                    async ([FromBody] PasswordResetConfirmDto dto, IAuthService authService) =>
                    {
                        try
                        {
                            await authService.ConfirmPasswordResetAsync(dto);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Password reset confirmed successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("ConfirmPasswordReset")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/users/invite
            app.MapPost(
                    "/api/v1/users/invite",
                    async (
                        [FromBody] InviteUserDto dto,
                        HttpContext context,
                        IAuthService authService,
                        IEmailService emailService
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var invitedUser = await authService.InviteUserAsync(
                                dto,
                                tenantId,
                                emailService
                            );
                            return Results.Ok(
                                new ApiResponse<UserDto>
                                {
                                    Data = invitedUser,
                                    Meta = new { Message = "User invited successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("InviteUser")
                .RequireAuthorization("AdminOnly")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // GET /api/v1/users
            app.MapGet(
                    "/api/v1/users",
                    async (
                        HttpContext context,
                        IAuthService authService,
                        [FromQuery] int page = 1,
                        [FromQuery] int limit = 50,
                        [FromQuery] string? sort = null,
                        [FromQuery] string? filter = null
                    ) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            var users = await authService.GetUsersAsync(
                                tenantId,
                                page,
                                limit,
                                sort,
                                filter
                            );
                            return Results.Ok(
                                new ApiResponse<PagedResult<UserDto>>
                                {
                                    Data = users,
                                    Meta = new
                                    {
                                        Page = page,
                                        Limit = limit,
                                        Total = users.TotalCount
                                    }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("GetUsers")
                .RequireAuthorization("AdminOnly")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // DELETE /api/v1/users/{userId}
            app.MapDelete(
                    "/api/v1/users/{userId}",
                    async (Guid userId, HttpContext context, IAuthService authService) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                            {
                                return Results.Unauthorized();
                            }
                            await authService.DeleteUserAsync(userId, tenantId);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "User deleted successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("DeleteUser")
                .RequireAuthorization("AdminOnly")
                .WithTags(AUTH_TAG)
                .WithMetadata();

            // POST /api/v1/auth/change-password
            app.MapPost(
                    "/api/v1/auth/change-password",
                    async (
                        [FromBody] ChangePasswordDto dto,
                        HttpContext context,
                        IAuthService authService
                    ) =>
                    {
                        try
                        {
                            var userId = dto.UserId;
                            var auuthenticated = (
                                (System.Security.Claims.ClaimsIdentity)context.User.Identity
                            ).IsAuthenticated;

                            if (auuthenticated == false || userId == Guid.Empty)
                            {
                                return Results.Unauthorized();
                            }

                            await authService.ChangePasswordAsync(userId, dto);
                            return Results.Ok(
                                new ApiResponse<object>
                                {
                                    Data = null,
                                    Meta = new { Message = "Password changed successfully" }
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(
                                new ApiResponse<object>
                                {
                                    Error = new ErrorResponse
                                    {
                                        Code = "BadRequest",
                                        Message = ex.Message
                                    }
                                }
                            );
                        }
                    }
                )
                .WithName("ChangePassword")
                .RequireAuthorization() // Restrict to authenticated users
                .WithTags(AUTH_TAG)
                .WithMetadata();
        }
    }
}
