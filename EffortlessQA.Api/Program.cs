using System.Reflection;
using System.Security.Claims;
using System.Text;
using EffortlessQA.Api.Extensions;
using EffortlessQA.Api.Extensions.Endpoints;
using EffortlessQA.Api.Middleware;
using EffortlessQA.Api.Services.Implementation;
using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

// Add EF Core with PostgreSQL
builder
    .Services.AddDbContext<EffortlessQAContext>(
        (sp, options) =>
        {
            var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EffortlessQA.Data")
            );
        }
    )
    .AddHttpContextAccessor();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"];
var keyBytes = Encoding.UTF8.GetBytes(keyString ?? "");
if (keyBytes.Length < 16)
{
    throw new InvalidOperationException(
        $"JWT key must be at least 16 bytes (128 bits) for HS256. Current length: {keyBytes.Length} bytes."
    );
}

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

// Authorization with RBAC
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleType.Admin.ToString()));
    options.AddPolicy(
        "TesterOrAdmin",
        policy => policy.RequireRole(RoleType.Tester.ToString(), RoleType.Admin.ToString())
    );
});

// FluentValidation
builder
    .Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITestSuiteService, TestSuiteService>();
builder.Services.AddScoped<ITestRunService, TestRunService>();
builder.Services.AddScoped<ITestRunResultService, TestRunResultService>();
builder.Services.AddScoped<IDefectService, DefectService>();
builder.Services.AddScoped<IRequirementService, RequirementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMiscellaneousService, MiscellaneousService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IPermissionRoleService, PermissionRoleService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITestCaseService, TestCaseService>();
builder.Services.AddScoped<ITestRunResultService, TestRunResultService>();
builder.Services.AddScoped<ITestFolderService, TestFolderService>();

builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSpecificOrigins",
        policy =>
        {
            var allowedOrigins = builder.Environment.IsDevelopment()
                ? new[] { "https://localhost:7129", "https://localhost:7129/" } // Allow both with/without trailing slash
                : new[] { "https://effortlessqa.netlify.app", "https://effortlessqa.netlify.app/" }; // Production

            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials(); // Required for authenticated requests
        }
    );
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "EffortlessQA API",
            Version = "v1",
            Description =
                "API for managing QA projects, test suites, test cases, test runs, defects, and requirements."
        }
    );
    // Temporarily disable security schemes

    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT token in the format **Bearer &lt;your_token&gt;**"
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        }
    );

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath, includeControllerXmlComments = true);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");

//app.UseMiddleware<TenantValidationMiddleware>(); // Temporarily disabled
app.UseMiddleware<RequestLoggingMiddleware>();

// Global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            Log.Error(exception, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(
                new ApiResponse<object>
                {
                    Error = new ErrorResponse
                    {
                        Code = "InternalServerError",
                        Message = app.Environment.IsDevelopment()
                            ? $"{exception.Message}\nStackTrace: {exception.StackTrace}"
                            : "An unexpected error occurred."
                    }
                }
            );
        }
    });
});

// Root endpoint for testing
app.MapGet("/", () => "EffortlessQA API is running. Access Swagger at /swagger.").WithName("Root");

app.MapApiEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EffortlessQA API v1"));
}

app.Run();
