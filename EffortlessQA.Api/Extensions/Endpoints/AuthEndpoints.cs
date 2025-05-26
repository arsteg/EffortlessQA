using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
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
                .WithTags(AUTH_TAG) // Added
                .WithMetadata();

            app.MapPost(
                    "/api/v1/auth/login",
                    async ([FromBody] LoginDto dto, IAuthService authService) =>
                    {
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
                .WithName("Login")
                .WithTags("Authentication") // Added
                .WithMetadata();
        }
    }
}
