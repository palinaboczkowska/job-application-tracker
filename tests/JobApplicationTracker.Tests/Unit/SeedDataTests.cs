using JobApplicationTracker.Domain.Data;
using JobApplicationTracker.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JobApplicationTracker.Tests.Unit;

public class SeedDataTests
{
    private static JobTrackerDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<JobTrackerDbContext>()
            .UseSqlite(connection)
            .Options;
        return new JobTrackerDbContext(options);
    }

    [Fact]
    public void EnsureSeeded_creates_one_application_per_status()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using var context = CreateContext(connection);
        context.Database.EnsureCreated();

        SeedData.EnsureSeeded(context);

        var statuses = context.JobApplications.Select(a => a.Status).ToList();
        Assert.Equal(5, statuses.Count);
        foreach (ApplicationStatus status in Enum.GetValues<ApplicationStatus>())
        {
            Assert.Contains(status, statuses);
        }
    }

    [Fact]
    public void EnsureSeeded_is_idempotent()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using var context = CreateContext(connection);
        context.Database.EnsureCreated();

        SeedData.EnsureSeeded(context);
        SeedData.EnsureSeeded(context);

        Assert.Equal(5, context.JobApplications.Count());
    }
}
