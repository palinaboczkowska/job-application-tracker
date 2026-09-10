namespace JobApplicationTracker.Contracts;

public class StatusChangeDto
{
    public string? FromStatus { get; set; }

    public required string ToStatus { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? Note { get; set; }

    public bool IsCorrection { get; set; }
}
