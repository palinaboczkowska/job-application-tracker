using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Contracts;

/// <summary>
/// Updates the editable fields only -- not Status, which has its own
/// dedicated endpoint and validation (see ChangeStatusRequest).
/// </summary>
public class UpdateJobApplicationRequest
{
    [Required]
    [MaxLength(200)]
    public required string CompanyName { get; set; }

    [Required]
    [MaxLength(200)]
    public required string RoleTitle { get; set; }

    [Url]
    [MaxLength(2048)]
    public string? JobPostingUrl { get; set; }

    public string? Notes { get; set; }
}
