using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static void MapMiscellaneousEndpoints(this WebApplication app)
        {
            // GET /api/v1/countries
            app.MapGet(
                    "/api/v1/countries",
                    async (IMiscellaneousService service) =>
                    {
                        try
                        {
                            var countries = await service.GetCountriesAsync();
                            return Results.Ok(
                                new ApiResponse<List<CountryDto>>
                                {
                                    Data = countries,
                                    Meta = new { Message = "Countries retrieved successfully" }
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
                .WithName("GetCountries")
                .WithTags(MISCELLANEOUS_TAG)
                .WithMetadata();

            // POST /api/v1/tenants/{tenantId}/address
            app.MapPost(
                    "/api/v1/tenants/{tenantId}/address",
                    async (
                        string tenantId,
                        [FromBody] CreateAddressDto dto,
                        IMiscellaneousService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var userTenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(userTenantId) || userTenantId != tenantId)
                                return Results.Unauthorized();
                            var address = await service.CreateTenantAddressAsync(tenantId, dto);
                            return Results.Ok(
                                new ApiResponse<AddressDto>
                                {
                                    Data = address,
                                    Meta = new { Message = "Address created successfully" }
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
                .WithName("CreateTenantAddress")
                .RequireAuthorization("AdminOnly")
                .WithTags(MISCELLANEOUS_TAG)
                .WithMetadata();

            // PUT /api/v1/tenants/{tenantId}/address
            app.MapPut(
                    "/api/v1/tenants/{tenantId}/address",
                    async (
                        string tenantId,
                        [FromBody] UpdateAddressDto dto,
                        IMiscellaneousService service,
                        HttpContext context
                    ) =>
                    {
                        try
                        {
                            var userTenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(userTenantId) || userTenantId != tenantId)
                                return Results.Unauthorized();
                            var address = await service.UpdateTenantAddressAsync(tenantId, dto);
                            return Results.Ok(
                                new ApiResponse<AddressDto>
                                {
                                    Data = address,
                                    Meta = new { Message = "Address updated successfully" }
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
                .WithName("UpdateTenantAddress")
                .RequireAuthorization("AdminOnly")
                .WithTags(MISCELLANEOUS_TAG)
                .WithMetadata();

            // GET /api/v1/setup/wizard
            app.MapGet(
                    "/api/v1/setup/wizard",
                    async (IMiscellaneousService service, HttpContext context) =>
                    {
                        try
                        {
                            var tenantId = context.User.FindFirst("TenantId")?.Value;
                            if (string.IsNullOrEmpty(tenantId))
                                return Results.Unauthorized();
                            var wizardData = await service.GetSetupWizardDataAsync(tenantId);
                            return Results.Ok(
                                new ApiResponse<SetupWizardDto>
                                {
                                    Data = wizardData,
                                    Meta = new
                                    {
                                        Message = "Setup wizard data retrieved successfully"
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
                .WithName("GetSetupWizard")
                .WithTags(MISCELLANEOUS_TAG)
                .WithMetadata();
        }
    }
}
