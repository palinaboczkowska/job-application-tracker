namespace JobApplicationTracker.Contracts;

/// <summary>
/// Status is a plain string here (the enum's name, e.g. "Interviewing"), not
/// a shared enum type -- Contracts has zero project references on purpose,
/// so it never pulls Domain's EF Core/SQLite dependency into the Blazor
/// WebAssembly client that also references this project. The API maps
/// between this string and the real ApplicationStatus enum at the boundary.
/// </summary>
public class JobApplicationDto
{
    public int Id { get; set; }

    public required string CompanyName { get; set; }

    public required string RoleTitle { get; set; }

    public required string Status { get; set; }

    public DateOnly AppliedOn { get; set; }

    public string? JobPostingUrl { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
