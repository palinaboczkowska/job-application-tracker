using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Domain.Data;

public class JobTrackerDbContext : DbContext
{
    public JobTrackerDbContext(DbContextOptions<JobTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    public DbSet<StatusChange> StatusChanges => Set<StatusChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.Property(a => a.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(a => a.RoleTitle).IsRequired().HasMaxLength(200);
            entity.Property(a => a.JobPostingUrl).HasMaxLength(2048);

            // Stored as text (e.g. "Interviewing"), not an int, so the raw
            // SQLite file stays human-legible when opened in any SQLite browser.
            entity.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);

            entity
                .HasMany(a => a.StatusChanges)
                .WithOne(c => c.JobApplication)
                .HasForeignKey(c => c.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StatusChange>(entity =>
        {
            entity.Property(c => c.FromStatus).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.ToStatus).HasConversion<string>().IsRequired().HasMaxLength(20);
        });
    }
}
