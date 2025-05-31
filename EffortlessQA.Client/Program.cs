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
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<AuthTokenHandler>();

builder
    .Services.AddHttpClient(
        "EffortlessQA.Api",
        client =>
        {
            client.BaseAddress = new Uri(
                builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7196/api/v1/"
            );
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

await builder.Build().RunAsync();
