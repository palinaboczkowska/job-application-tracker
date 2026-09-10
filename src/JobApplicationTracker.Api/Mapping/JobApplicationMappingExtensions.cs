using JobApplicationTracker.Contracts;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Api.Mapping;

public static class JobApplicationMappingExtensions
{
    public static JobApplicationDto ToDto(this JobApplication entity) => new()
    {
        Id = entity.Id,
        CompanyName = entity.CompanyName,
        RoleTitle = entity.RoleTitle,
        Status = entity.Status.ToString(),
        AppliedOn = entity.AppliedOn,
        JobPostingUrl = entity.JobPostingUrl,
        Notes = entity.Notes,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
    };

    public static StatusChangeDto ToDto(this StatusChange entity) => new()
    {
        FromStatus = entity.FromStatus?.ToString(),
        ToStatus = entity.ToStatus.ToString(),
        ChangedAt = entity.ChangedAt,
        Note = entity.Note,
        IsCorrection = entity.IsCorrection,
    };
}
