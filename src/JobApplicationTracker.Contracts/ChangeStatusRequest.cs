using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Contracts;

public class ChangeStatusRequest
{
    [Required]
    public required string NewStatus { get; set; }

    public string? Note { get; set; }
}
