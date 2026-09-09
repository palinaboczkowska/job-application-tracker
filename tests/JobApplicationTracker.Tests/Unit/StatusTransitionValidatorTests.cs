using JobApplicationTracker.Domain.Entities;
using JobApplicationTracker.Domain.Services;
using Xunit;

namespace JobApplicationTracker.Tests.Unit;

public class StatusTransitionValidatorTests
{
    [Theory]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.Interviewing)]
    [InlineData(ApplicationStatus.Interviewing, ApplicationStatus.Offer)]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.Offer)] // skipping a stage is allowed
    public void Forward_or_skip_ahead_moves_are_valid(ApplicationStatus from, ApplicationStatus to)
    {
        Assert.True(StatusTransitionValidator.IsValidTransition(from, to));
    }

    [Theory]
    [InlineData(ApplicationStatus.Offer, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.Offer, ApplicationStatus.Interviewing)]
    [InlineData(ApplicationStatus.Interviewing, ApplicationStatus.Applied)]
    public void Backward_moves_are_invalid(ApplicationStatus from, ApplicationStatus to)
    {
        Assert.False(StatusTransitionValidator.IsValidTransition(from, to));
    }

    [Theory]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Interviewing, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Offer, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.Withdrawn)]
    [InlineData(ApplicationStatus.Interviewing, ApplicationStatus.Withdrawn)]
    [InlineData(ApplicationStatus.Offer, ApplicationStatus.Withdrawn)]
    public void Any_non_terminal_status_can_move_to_a_terminal_status(
        ApplicationStatus from,
        ApplicationStatus to)
    {
        Assert.True(StatusTransitionValidator.IsValidTransition(from, to));
    }

    [Theory]
    [InlineData(ApplicationStatus.Rejected, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.Rejected, ApplicationStatus.Interviewing)]
    [InlineData(ApplicationStatus.Rejected, ApplicationStatus.Withdrawn)]
    [InlineData(ApplicationStatus.Withdrawn, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.Withdrawn, ApplicationStatus.Rejected)]
    public void Terminal_statuses_cannot_be_left(ApplicationStatus from, ApplicationStatus to)
    {
        Assert.False(StatusTransitionValidator.IsValidTransition(from, to));
    }

    [Theory]
    [InlineData(ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.Offer)]
    [InlineData(ApplicationStatus.Rejected)]
    public void A_status_cannot_transition_to_itself(ApplicationStatus status)
    {
        Assert.False(StatusTransitionValidator.IsValidTransition(status, status));
    }
}
