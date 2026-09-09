using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Contracts;

/// <summary>
/// No Status field on purpose -- every new application starts as "Applied".
/// This removes an entire class of edge cases (there's no such thing as
/// "create an application that's already Interviewing").
/// </summary>
public class CreateJobApplicationRequest
{
    [Required]
    [MaxLength(200)]
    public required string CompanyName { get; set; }

    [Required]
    [MaxLength(200)]
    public required string RoleTitle { get; set; }

    public DateOnly AppliedOn { get; set; }

    [Url]
    [MaxLength(2048)]
    public string? JobPostingUrl { get; set; }

    public string? Notes { get; set; }
}
