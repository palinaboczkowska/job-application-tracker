using JobApplicationTracker.Api.Mapping;
using JobApplicationTracker.Contracts;
using JobApplicationTracker.Domain.Data;
using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Api.Controllers;

[ApiController]
[Route("api/job-applications")]
public class JobApplicationsController : ControllerBase
{
    private readonly JobTrackerDbContext _context;

    public JobApplicationsController(JobTrackerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetAll([FromQuery] string? status)
    {
        var query = _context.JobApplications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<ApplicationStatus>(status, ignoreCase: true, out var parsedStatus))
            {
                return BadRequest(new { error = $"Unknown status '{status}'." });
            }
            query = query.Where(a => a.Status == parsedStatus);
        }

        var applications = await query.OrderByDescending(a => a.UpdatedAt).ToListAsync();
        return Ok(applications.Select(a => a.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobApplicationDto>> GetById(int id)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application is null)
        {
            return NotFound();
        }
        return Ok(application.ToDto());
    }

    [HttpGet("{id:int}/status-history")]
    public async Task<ActionResult<IEnumerable<StatusChangeDto>>> GetStatusHistory(int id)
    {
        var exists = await _context.JobApplications.AnyAsync(a => a.Id == id);
        if (!exists)
        {
            return NotFound();
        }

        var history = await _context.StatusChanges
            .Where(c => c.JobApplicationId == id)
            .OrderBy(c => c.ChangedAt)
            .ToListAsync();

        return Ok(history.Select(c => c.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationDto>> Create(CreateJobApplicationRequest request)
    {
        var now = DateTime.UtcNow;
        var application = new JobApplication
        {
            CompanyName = request.CompanyName,
            RoleTitle = request.RoleTitle,
            Status = ApplicationStatus.Applied,
            AppliedOn = request.AppliedOn == default ? DateOnly.FromDateTime(now) : request.AppliedOn,
            JobPostingUrl = request.JobPostingUrl,
            Notes = request.Notes,
            CreatedAt = now,
            UpdatedAt = now,
        };
        application.StatusChanges.Add(new StatusChange
        {
            FromStatus = null,
            ToStatus = ApplicationStatus.Applied,
            ChangedAt = now,
        });

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = application.Id }, application.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateJobApplicationRequest request)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application is null)
        {
            return NotFound();
        }

        application.CompanyName = request.CompanyName;
        application.RoleTitle = request.RoleTitle;
        application.JobPostingUrl = request.JobPostingUrl;
        application.Notes = request.Notes;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<JobApplicationDto>> ChangeStatus(int id, ChangeStatusRequest request)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application is null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<ApplicationStatus>(request.NewStatus, ignoreCase: true, out var newStatus))
        {
            return BadRequest(new { error = $"Unknown status '{request.NewStatus}'." });
        }

        if (!StatusTransitionValidator.IsValidTransition(application.Status, newStatus))
        {
            return Conflict(new
            {
                error = $"Cannot transition from '{application.Status}' to '{newStatus}'.",
            });
        }

        var now = DateTime.UtcNow;
        _context.StatusChanges.Add(new StatusChange
        {
            JobApplicationId = application.Id,
            FromStatus = application.Status,
            ToStatus = newStatus,
            ChangedAt = now,
            Note = request.Note,
        });
        application.Status = newStatus;
        application.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return Ok(application.ToDto());
    }

    /// <summary>
    /// Sets the status directly, bypassing <see cref="StatusTransitionValidator"/>
    /// entirely. For fixing a data-entry mistake (e.g. the wrong button was
    /// clicked) -- not a substitute for the validated <see cref="ChangeStatus"/>
    /// endpoint, which remains the normal way to move an application forward.
    /// </summary>
    [HttpPatch("{id:int}/status/correct")]
    public async Task<ActionResult<JobApplicationDto>> CorrectStatus(int id, ChangeStatusRequest request)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application is null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<ApplicationStatus>(request.NewStatus, ignoreCase: true, out var newStatus))
        {
            return BadRequest(new { error = $"Unknown status '{request.NewStatus}'." });
        }

        if (newStatus == application.Status)
        {
            return BadRequest(new { error = $"Already at status '{newStatus}'." });
        }

        var now = DateTime.UtcNow;
        _context.StatusChanges.Add(new StatusChange
        {
            JobApplicationId = application.Id,
            FromStatus = application.Status,
            ToStatus = newStatus,
            ChangedAt = now,
            Note = request.Note,
            IsCorrection = true,
        });
        application.Status = newStatus;
        application.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return Ok(application.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var application = await _context.JobApplications.FindAsync(id);
        if (application is null)
        {
            return NotFound();
        }

        _context.JobApplications.Remove(application);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
