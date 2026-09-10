using JobApplicationTracker.Contracts;

namespace JobApplicationTracker.Client.Services;

/// <summary>
/// The result of a status-change attempt. A failed transition is a normal,
/// expected outcome (the API answers 409 Conflict for it) -- not an
/// exception -- so components can show a friendly message instead of
/// crashing.
/// </summary>
public class StatusChangeResult
{
    public required bool Success { get; init; }

    public JobApplicationDto? Application { get; init; }
}

public interface IJobApplicationApiClient
{
    Task<List<JobApplicationDto>> GetAllAsync(string? status = null);

    Task<JobApplicationDto?> GetByIdAsync(int id);

    Task<List<StatusChangeDto>> GetStatusHistoryAsync(int id);

    Task<JobApplicationDto> CreateAsync(CreateJobApplicationRequest request);

    Task UpdateAsync(int id, UpdateJobApplicationRequest request);

    Task<StatusChangeResult> ChangeStatusAsync(int id, ChangeStatusRequest request);

    Task DeleteAsync(int id);
}
