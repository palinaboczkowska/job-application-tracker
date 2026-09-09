using JobApplicationTracker.Domain.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string BlazorClientCorsPolicy = "BlazorClient";

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<JobTrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("JobTrackerDb")
        ?? "Data Source=jobtracker.db"));

// Needed specifically because the frontend is Blazor WebAssembly (runs in
// the browser, makes real cross-origin fetch calls) rather than Blazor
// Server (which never leaves the server process). Allowed origins come from
// configuration, not a wildcard -- see appsettings.json's "Cors" section.
// Exact dev port confirmed once the Client project exists (Session 3).
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? Array.Empty<string>();
    options.AddPolicy(BlazorClientCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<JobTrackerDbContext>();
    context.Database.Migrate();
    SeedData.EnsureSeeded(context);
}

app.UseHttpsRedirection();
app.UseCors(BlazorClientCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposes the implicit Program class to the test project's
// WebApplicationFactory<Program> -- top-level statements generate an
// `internal` Program class by default, which is invisible from a separate
// test assembly without this.
public partial class Program { }
