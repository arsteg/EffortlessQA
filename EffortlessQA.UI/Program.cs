using Blazored.LocalStorage;
using EffortlessQA.UI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    //options.MaxMessageSize = 10 * 1024 * 1024; // 10MB for SignalR messages
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5); // Retain circuit for 5 minutes
    options.DisconnectedCircuitMaxRetained = 100; // Max retained circuits
});
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient(
    "EffortlessQAApi",
    client =>
    {
        client.BaseAddress = new Uri("https://localhost:7196/api/v1/");
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
        );
        client.Timeout = TimeSpan.FromSeconds(60); // Increase HTTP timeout for image uploads
    }
).AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB for SignalR messages
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(120); // Client timeout
    options.KeepAliveInterval = TimeSpan.FromSeconds(60); // Keep-alive ping
    options.EnableDetailedErrors = true; // Enable detailed SignalR errors for debugging
});

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

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB for request body
    //options.Limits.RequestBufferSize = 10 * 1024 * 1024; // 10MB buffer
});

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

app.MapBlazorHub(options =>
{
    options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(10);
    options.LongPolling.PollTimeout = TimeSpan.FromSeconds(60);
});
app.MapFallbackToPage("/_Host");

app.Run();