using Domain.Entities;
using Domain.Enums;

public class InterviewStageTests
{
    private static readonly Guid ApplicationId = Guid.NewGuid();

    private static InterviewStage CreateValidInterviewStage()
    {
        return new InterviewStage(
            applicationId: ApplicationId,
            stageType: InterviewStageType.TechnicalInterview);
    }

    // Testiranje konstruktora
    [Fact]
    public void Constructor_InvalidApplicationId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new InterviewStage(
            applicationId: Guid.Empty,
            stageType: InterviewStageType.Screening));
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesInterviewStage()
    {
        var stage = CreateValidInterviewStage();

        Assert.NotEqual(Guid.Empty, stage.Id);
        Assert.Equal(ApplicationId, stage.ApplicationId);
        Assert.Equal(
            InterviewStageType.TechnicalInterview,
            stage.StageType);
        Assert.Equal(InterviewOutcome.Pending, stage.Outcome);
        Assert.Null(stage.ScheduledAt);
        Assert.Null(stage.CompletedAt);
        Assert.Null(stage.Notes);
    }

    // Testiranje Schedule() metode
    [Fact]
    public void Schedule_WhenDateIsInFuture_SetsScheduledAt()
    {
        var stage = CreateValidInterviewStage();

        var scheduledAt = DateTime.UtcNow.AddDays(2);

        stage.Schedule(scheduledAt);

        Assert.Equal(scheduledAt, stage.ScheduledAt);
    }

    [Fact]
    public void Schedule_WhenDateIsInPast_ThrowsArgumentException()
    {
        var stage = CreateValidInterviewStage();

        Assert.Throws<ArgumentException>(() =>
            stage.Schedule(DateTime.UtcNow.AddMinutes(-1)));
    }

    [Fact]
    public void Schedule_WhenDateIsNow_ThrowsArgumentException()
    {
        var stage = CreateValidInterviewStage();

        Assert.Throws<ArgumentException>(() =>
            stage.Schedule(DateTime.UtcNow));
    }

    [Fact]
    public void Schedule_WhenStageIsPassed_ThrowsInvalidOperationException()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(InterviewOutcome.Passed);

        Assert.Throws<InvalidOperationException>(() =>
            stage.Schedule(DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Schedule_WhenStageIsFailed_ThrowsInvalidOperationException()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(InterviewOutcome.Failed);

        Assert.Throws<InvalidOperationException>(() =>
            stage.Schedule(DateTime.UtcNow.AddDays(1)));
    }

    // Testiranje Complete() metode
    [Fact]
    public void Complete_WhenOutcomeIsPending_ThrowsArgumentException()
    {
        var stage = CreateValidInterviewStage();

        Assert.Throws<ArgumentException>(() =>
            stage.Complete(InterviewOutcome.Pending));
    }

    [Fact]
    public void Complete_WhenOutcomeIsPassed_SetsOutcome()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(InterviewOutcome.Passed);

        Assert.Equal(InterviewOutcome.Passed, stage.Outcome);
    }

    [Fact]
    public void Complete_WhenOutcomeIsFailed_SetsOutcome()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(InterviewOutcome.Failed);

        Assert.Equal(InterviewOutcome.Failed, stage.Outcome);
    }

    [Fact]
    public void Complete_WhenOutcomeIsPassed_SetsCompletedAt()
    {
        var stage = CreateValidInterviewStage();

        var before = DateTime.UtcNow;

        stage.Complete(InterviewOutcome.Passed);

        var after = DateTime.UtcNow;

        Assert.NotNull(stage.CompletedAt);
        Assert.InRange(stage.CompletedAt.Value, before, after);
    }

    [Fact]
    public void Complete_WhenNotesAreProvided_SetsNotes()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(
            InterviewOutcome.Passed,
            "Kandidat je pokazao odlično tehničko znanje.");

        Assert.Equal(
            "Kandidat je pokazao odlično tehničko znanje.",
            stage.Notes);
    }

    [Fact]
    public void Complete_WhenNotesAreNotProvided_LeavesNotesNull()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(InterviewOutcome.Passed);

        Assert.Null(stage.Notes);
    }

    [Fact]
    public void Complete_WhenStageIsAlreadyPassed_ThrowsInvalidOperationException()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(InterviewOutcome.Passed);

        Assert.Throws<InvalidOperationException>(() =>
            stage.Complete(InterviewOutcome.Failed));
    }

    [Fact]
    public void Complete_WhenStageIsAlreadyFailed_ThrowsInvalidOperationException()
    {
        var stage = CreateValidInterviewStage();

        stage.Complete(InterviewOutcome.Failed);

        Assert.Throws<InvalidOperationException>(() =>
            stage.Complete(InterviewOutcome.Passed));
    }

    [Fact]
    public void Complete_WhenStageIsScheduled_SetsCompletedAt()
    {
        var stage = CreateValidInterviewStage();

        stage.Schedule(DateTime.UtcNow.AddDays(1));

        stage.Complete(
            InterviewOutcome.Passed,
            "Intervju uspješno završen.");

        Assert.Equal(InterviewOutcome.Passed, stage.Outcome);
        Assert.NotNull(stage.CompletedAt);
        Assert.Equal(
            "Intervju uspješno završen.",
            stage.Notes);
    }
}