namespace JobApplicationTracker.Domain.Entities;

/// <summary>
/// One entry in a job application's status timeline. <see cref="FromStatus"/>
/// is null only for the very first entry, recorded at creation.
/// </summary>
public class StatusChange
{
    public int Id { get; set; }

    public int JobApplicationId { get; set; }

    public JobApplication? JobApplication { get; set; }

    public ApplicationStatus? FromStatus { get; set; }

    public ApplicationStatus ToStatus { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? Note { get; set; }
}
