using System.Text;
using EffortlessQA.Data;
using EffortlessQA.Data.Dtos;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core with PostgreSQL and specify migrations assembly
builder
    .Services.AddDbContext<EffortlessQAContext>(
        (sp, options) =>
        {
            var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EffortlessQA.Data")
            ); // Specify migrations assembly
        }
    )
    .AddHttpContextAccessor();

// Add Authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Example endpoint
app.MapPost(
    "/api/testsuites",
    async (EffortlessQAContext db, TestSuiteCreateDto dto) =>
    {
        var testSuite = new TestSuite
        {
            Name = dto.Name,

            ProjectId = dto.ProjectId,
            TenantId = dto.TenantId
        };
        db.TestSuites.Add(testSuite);
        await db.SaveChangesAsync();
        return Results.Created($"/api/testsuites/{testSuite.Id}", testSuite);
    }
); // Temporarily remove .RequireAuthorization() for testing

app.Run();
