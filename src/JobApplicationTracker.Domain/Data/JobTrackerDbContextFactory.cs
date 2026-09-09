using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobApplicationTracker.Domain.Data;

/// <summary>
/// Lets `dotnet ef` create migrations against this class library directly,
/// without needing the API project (or any host) to exist yet. Only used at
/// design time (migrations); the real app configures its own connection
/// string via dependency injection in Program.cs.
/// </summary>
public class JobTrackerDbContextFactory : IDesignTimeDbContextFactory<JobTrackerDbContext>
{
    public JobTrackerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<JobTrackerDbContext>();
        optionsBuilder.UseSqlite("Data Source=jobtracker.db");
        return new JobTrackerDbContext(optionsBuilder.Options);
    }
}
