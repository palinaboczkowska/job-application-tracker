using JobApplicationTracker.Domain.Data;
using JobApplicationTracker.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JobApplicationTracker.Tests.Unit;

/// <summary>
/// Sanity check flagged in the plan: DateOnly has mapped cleanly to SQLite
/// TEXT since EF Core 8, but this project runs on EF Core 10 -- confirmed
/// here with one real round trip rather than assumed.
/// </summary>
public class DateOnlyRoundTripTests
{
    [Fact]
    public void AppliedOn_round_trips_through_sqlite_unchanged()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<JobTrackerDbContext>()
            .UseSqlite(connection)
            .Options;

        var expectedDate = new DateOnly(2026, 3, 15);

        using (var context = new JobTrackerDbContext(options))
        {
            context.Database.EnsureCreated();
            context.JobApplications.Add(new JobApplication
            {
                CompanyName = "Northwind Analytics",
                RoleTitle = "Backend Developer",
                AppliedOn = expectedDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
            context.SaveChanges();
        }

        using (var freshContext = new JobTrackerDbContext(options))
        {
            var reloaded = freshContext.JobApplications.Single();
            Assert.Equal(expectedDate, reloaded.AppliedOn);
        }
    }
}
