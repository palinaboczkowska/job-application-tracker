using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Domain.Data;

/// <summary>
/// Entirely invented sample data using classic Microsoft-tutorial placeholder
/// company names (Northwind, Contoso, Fabrikam) -- deliberately, since they
/// read as unambiguously fictional to anyone with .NET familiarity, on top of
/// simply not being real. Seeded at runtime (not via HasData) so the sample
/// set can be edited freely without generating a new migration every time --
/// see docs/ef-core-seeding-design.md for the full reasoning.
/// </summary>
public static class SeedData
{
    public static void EnsureSeeded(JobTrackerDbContext context)
    {
        if (context.JobApplications.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var applied = new JobApplication
        {
            CompanyName = "Northwind Analytics",
            RoleTitle = "Backend Developer",
            Status = ApplicationStatus.Applied,
            AppliedOn = DateOnly.FromDateTime(now.AddDays(-10)),
            JobPostingUrl = "https://example.com/jobs/northwind-backend-developer",
            CreatedAt = now.AddDays(-10),
            UpdatedAt = now.AddDays(-10),
        };
        applied.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = ApplicationStatus.Applied,
            ChangedAt = now.AddDays(-10),
        });

        var interviewing = new JobApplication
        {
            CompanyName = "Contoso Cloud Services",
            RoleTitle = ".NET Developer",
            Status = ApplicationStatus.Interviewing,
            AppliedOn = DateOnly.FromDateTime(now.AddDays(-21)),
            CreatedAt = now.AddDays(-21),
            UpdatedAt = now.AddDays(-3),
        };
        interviewing.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = ApplicationStatus.Applied,
            ChangedAt = now.AddDays(-21),
        });
        interviewing.StatusChanges.Add(new StatusChange
        {
            FromStatus = ApplicationStatus.Applied,
            ToStatus = ApplicationStatus.Interviewing,
            ChangedAt = now.AddDays(-3),
            Note = "First-round interview scheduled",
        });

        var offer = new JobApplication
        {
            CompanyName = "Fabrikam Retail Group",
            RoleTitle = "Full-Stack Engineer",
            Status = ApplicationStatus.Offer,
            AppliedOn = DateOnly.FromDateTime(now.AddDays(-35)),
            Notes = "Offer received, considering.",
            CreatedAt = now.AddDays(-35),
            UpdatedAt = now.AddDays(-1),
        };
        offer.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = ApplicationStatus.Applied,
            ChangedAt = now.AddDays(-35),
        });
        offer.StatusChanges.Add(new StatusChange
        {
            FromStatus = ApplicationStatus.Applied,
            ToStatus = ApplicationStatus.Interviewing,
            ChangedAt = now.AddDays(-20),
        });
        offer.StatusChanges.Add(new StatusChange
        {
            FromStatus = ApplicationStatus.Interviewing,
            ToStatus = ApplicationStatus.Offer,
            ChangedAt = now.AddDays(-1),
            Note = "Verbal offer, written offer to follow",
        });

        var rejected = new JobApplication
        {
            CompanyName = "Adventure Works Logistics",
            RoleTitle = "Junior Software Engineer",
            Status = ApplicationStatus.Rejected,
            AppliedOn = DateOnly.FromDateTime(now.AddDays(-40)),
            CreatedAt = now.AddDays(-40),
            UpdatedAt = now.AddDays(-30),
        };
        rejected.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = ApplicationStatus.Applied,
            ChangedAt = now.AddDays(-40),
        });
        rejected.StatusChanges.Add(new StatusChange
        {
            FromStatus = ApplicationStatus.Applied,
            ToStatus = ApplicationStatus.Rejected,
            ChangedAt = now.AddDays(-30),
            Note = "Position filled internally",
        });

        var withdrawn = new JobApplication
        {
            CompanyName = "Tailspin Toys",
            RoleTitle = "Software Engineer",
            Status = ApplicationStatus.Withdrawn,
            AppliedOn = DateOnly.FromDateTime(now.AddDays(-15)),
            Notes = "Withdrew after accepting a different offer.",
            CreatedAt = now.AddDays(-15),
            UpdatedAt = now.AddDays(-2),
        };
        withdrawn.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = ApplicationStatus.Applied,
            ChangedAt = now.AddDays(-15),
        });
        withdrawn.StatusChanges.Add(new StatusChange
        {
            FromStatus = ApplicationStatus.Applied,
            ToStatus = ApplicationStatus.Withdrawn,
            ChangedAt = now.AddDays(-2),
        });

        context.JobApplications.AddRange(applied, interviewing, offer, rejected, withdrawn);
        context.SaveChanges();
    }
}
