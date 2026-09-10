using System.Net;
using System.Net.Http.Json;
using JobApplicationTracker.Contracts;

namespace JobApplicationTracker.Client.Services;

public class JobApplicationApiClient : IJobApplicationApiClient
{
    private readonly HttpClient _http;

    public JobApplicationApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<JobApplicationDto>> GetAllAsync(string? status = null)
    {
        var url = "api/job-applications";
        if (!string.IsNullOrWhiteSpace(status))
        {
            url += $"?status={Uri.EscapeDataString(status)}";
        }

        return await _http.GetFromJsonAsync<List<JobApplicationDto>>(url) ?? [];
    }

    public async Task<JobApplicationDto?> GetByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/job-applications/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JobApplicationDto>();
    }

    public async Task<List<StatusChangeDto>> GetStatusHistoryAsync(int id)
    {
        return await _http.GetFromJsonAsync<List<StatusChangeDto>>($"api/job-applications/{id}/status-history") ?? [];
    }

    public async Task<JobApplicationDto> CreateAsync(CreateJobApplicationRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/job-applications", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JobApplicationDto>())!;
    }

    public async Task UpdateAsync(int id, UpdateJobApplicationRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/job-applications/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<StatusChangeResult> ChangeStatusAsync(int id, ChangeStatusRequest request)
    {
        var response = await _http.PatchAsJsonAsync($"api/job-applications/{id}/status", request);
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return new StatusChangeResult { Success = false };
        }

        response.EnsureSuccessStatusCode();
        var application = await response.Content.ReadFromJsonAsync<JobApplicationDto>();
        return new StatusChangeResult { Success = true, Application = application };
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/job-applications/{id}");
        response.EnsureSuccessStatusCode();
    }
}
