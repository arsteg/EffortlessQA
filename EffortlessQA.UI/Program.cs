using EffortlessQA.UI.Services; // Ensure this namespace matches your project
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(); // Add MudBlazor services

// Register IHttpClientFactory with named client
builder.Services.AddHttpClient(
    "EffortlessQAApi",
    client =>
    {
        client.BaseAddress = new Uri("https://api.effortlessqa.com");
    }
);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<TestSuiteService>();
builder.Services.AddScoped<TestCaseService>();
builder.Services.AddScoped<TestRunService>();
builder.Services.AddScoped<DefectService>();
builder.Services.AddScoped<ReportingService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<RequirementService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<CommonService>();
builder.Services.AddScoped<ApplicationContextService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
