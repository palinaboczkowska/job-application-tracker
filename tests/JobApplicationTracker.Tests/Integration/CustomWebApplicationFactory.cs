using JobApplicationTracker.Domain.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplicationTracker.Tests.Integration;

/// <summary>
/// Swaps the real file-based SQLite connection for a kept-open in-memory
/// one. The connection must stay open for this factory's whole lifetime --
/// a `:memory:` SQLite database is destroyed the instant its last open
/// connection closes, and EF Core opens/closes connections per operation by
/// default, so a plain connection *string* would give every DbContext an
/// independent, empty database. This override runs before Program.cs's own
/// `builder.Build()` executes, so its migrate+seed calls apply to this
/// in-memory database automatically.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public CustomWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<JobTrackerDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<JobTrackerDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
