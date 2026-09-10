using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using JobApplicationTracker.Client;
using JobApplicationTracker.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5097";
builder.Services.AddScoped<IJobApplicationApiClient>(_ => new JobApplicationApiClient(
    new HttpClient { BaseAddress = new Uri(apiBaseUrl) }));
builder.Services.AddScoped<ILanguageService, LanguageService>();

var host = builder.Build();

// Load the saved language before the first render so there's no flash of the default language.
await host.Services.GetRequiredService<ILanguageService>().InitializeAsync();

await host.RunAsync();
