namespace JobApplicationTracker.Domain.Entities;

public class JobApplication
{
    public int Id { get; set; }

    public required string CompanyName { get; set; }

    public required string RoleTitle { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

    public DateOnly AppliedOn { get; set; }

    public string? JobPostingUrl { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<StatusChange> StatusChanges { get; set; } = new List<StatusChange>();
}
