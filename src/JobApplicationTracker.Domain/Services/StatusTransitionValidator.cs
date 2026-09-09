using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Domain.Services;

/// <summary>
/// Rules: Rejected and Withdrawn are terminal (no transition out of them).
/// From any non-terminal status, moving to Rejected or Withdrawn is always
/// allowed. Among the progression statuses (Applied &lt; Interviewing &lt; Offer),
/// only strictly forward moves are valid -- skipping a stage is fine (real
/// hiring processes skip stages), moving backward is not.
/// </summary>
public static class StatusTransitionValidator
{
    private static readonly ApplicationStatus[] ProgressionOrder =
    {
        ApplicationStatus.Applied,
        ApplicationStatus.Interviewing,
        ApplicationStatus.Offer,
    };

    private static readonly HashSet<ApplicationStatus> TerminalStatuses = new()
    {
        ApplicationStatus.Rejected,
        ApplicationStatus.Withdrawn,
    };

    public static bool IsValidTransition(ApplicationStatus from, ApplicationStatus to)
    {
        if (from == to)
        {
            return false;
        }

        if (TerminalStatuses.Contains(from))
        {
            return false;
        }

        if (TerminalStatuses.Contains(to))
        {
            return true;
        }

        var fromIndex = Array.IndexOf(ProgressionOrder, from);
        var toIndex = Array.IndexOf(ProgressionOrder, to);
        return toIndex > fromIndex;
    }
}
