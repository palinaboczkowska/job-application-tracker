using System.Net;
using System.Net.Http.Json;
using JobApplicationTracker.Contracts;
using Xunit;

namespace JobApplicationTracker.Tests.Integration;

public class JobApplicationsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public JobApplicationsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_returns_the_seeded_applications()
    {
        var response = await _client.GetAsync("/api/job-applications");
        response.EnsureSuccessStatusCode();

        var applications = await response.Content.ReadFromJsonAsync<List<JobApplicationDto>>();
        Assert.NotNull(applications);
        Assert.True(applications!.Count >= 5, "Expected at least the 5 seeded applications.");
    }

    [Fact]
    public async Task GetById_returns_404_for_a_missing_id()
    {
        var response = await _client.GetAsync("/api/job-applications/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_returns_201_with_Location_and_starts_as_Applied()
    {
        var request = new CreateJobApplicationRequest
        {
            CompanyName = "Test Co",
            RoleTitle = "Test Role",
            AppliedOn = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var response = await _client.PostAsJsonAsync("/api/job-applications", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<JobApplicationDto>();
        Assert.NotNull(created);
        Assert.Equal("Applied", created!.Status);

        var getResponse = await _client.GetAsync(response.Headers.Location);
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ChangeStatus_with_a_valid_transition_updates_status_and_history()
    {
        var created = await CreateApplicationAsync();

        var response = await _client.PatchAsJsonAsync(
            $"/api/job-applications/{created.Id}/status",
            new ChangeStatusRequest { NewStatus = "Interviewing" });

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<JobApplicationDto>();
        Assert.Equal("Interviewing", updated!.Status);

        var historyResponse = await _client.GetAsync($"/api/job-applications/{created.Id}/status-history");
        historyResponse.EnsureSuccessStatusCode();
        var history = await historyResponse.Content.ReadFromJsonAsync<List<StatusChangeDto>>();
        Assert.Equal(2, history!.Count); // creation (null -> Applied) + this change
    }

    [Fact]
    public async Task ChangeStatus_with_an_invalid_transition_returns_409()
    {
        var created = await CreateApplicationAsync();

        var response = await _client.PatchAsJsonAsync(
            $"/api/job-applications/{created.Id}/status",
            new ChangeStatusRequest { NewStatus = "Offer" });
        response.EnsureSuccessStatusCode(); // Applied -> Offer is a valid skip-ahead move

        // Offer -> Applied is a backward move: invalid.
        var backwardResponse = await _client.PatchAsJsonAsync(
            $"/api/job-applications/{created.Id}/status",
            new ChangeStatusRequest { NewStatus = "Applied" });

        Assert.Equal(HttpStatusCode.Conflict, backwardResponse.StatusCode);
    }

    [Fact]
    public async Task CorrectStatus_bypasses_validation_and_flags_the_history_entry()
    {
        var created = await CreateApplicationAsync();

        // Applied -> Rejected is a normal, valid transition.
        var toRejected = await _client.PatchAsJsonAsync(
            $"/api/job-applications/{created.Id}/status",
            new ChangeStatusRequest { NewStatus = "Rejected" });
        toRejected.EnsureSuccessStatusCode();

        // Rejected -> Applied would be refused by the validated endpoint
        // (Rejected is terminal), but the correction endpoint bypasses that.
        var corrected = await _client.PatchAsJsonAsync(
            $"/api/job-applications/{created.Id}/status/correct",
            new ChangeStatusRequest { NewStatus = "Applied", Note = "Fixed a mis-click" });

        corrected.EnsureSuccessStatusCode();
        var updated = await corrected.Content.ReadFromJsonAsync<JobApplicationDto>();
        Assert.Equal("Applied", updated!.Status);

        var history = await _client.GetFromJsonAsync<List<StatusChangeDto>>(
            $"/api/job-applications/{created.Id}/status-history");
        var correctionEntry = Assert.Single(history!, c => c.IsCorrection);
        Assert.Equal("Rejected", correctionEntry.FromStatus);
        Assert.Equal("Applied", correctionEntry.ToStatus);
    }

    [Fact]
    public async Task CorrectStatus_rejects_setting_the_same_status()
    {
        var created = await CreateApplicationAsync();

        var response = await _client.PatchAsJsonAsync(
            $"/api/job-applications/{created.Id}/status/correct",
            new ChangeStatusRequest { NewStatus = "Applied" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_removes_the_application()
    {
        var created = await CreateApplicationAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/job-applications/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/job-applications/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<JobApplicationDto> CreateApplicationAsync()
    {
        var request = new CreateJobApplicationRequest
        {
            CompanyName = "Test Co",
            RoleTitle = "Test Role",
            AppliedOn = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        var response = await _client.PostAsJsonAsync("/api/job-applications", request);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<JobApplicationDto>();
        return created!;
    }
}
