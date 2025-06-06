using System.Net.Http.Headers;
using Blazored.LocalStorage;
using EffortlessQA.Client;
using EffortlessQA.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

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
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<AuthTokenHandler>();

// Determine the environment and select the appropriate API URL
var apiBaseUrl = builder.HostEnvironment.IsDevelopment()
    ? builder.Configuration["ApiBaseUrl:Local"] ?? "https://localhost:7196/api/v1/"
    : builder.Configuration["ApiBaseUrl:Production"]
        ?? "https://effortlessqa-api-a9606f6fb190.herokuapp.com/api/v1/";

builder
    .Services.AddHttpClient(
        "EffortlessQA.Api",
        client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }
    )
    .AddHttpMessageHandler<AuthTokenHandler>();

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

await builder.Build().RunAsync();
