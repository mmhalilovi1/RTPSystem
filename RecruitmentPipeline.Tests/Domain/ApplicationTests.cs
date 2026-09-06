using Domain.Entities;
using Domain.Enums;

public class ApplicationTests
{
    private static readonly Guid CandidateId = Guid.NewGuid();
    private static readonly Guid PositionId = Guid.NewGuid();

    private static Domain.Entities.Application CreateValidApplication()
    {
        return new Domain.Entities.Application(
            candidateId: CandidateId,
            positionId: PositionId);
    }

    // Testiranje konstruktora
    [Fact]
    public void Constructor_InvalidCandidateId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Domain.Entities.Application(
            candidateId: Guid.Empty,
            positionId: PositionId));
    }

    [Fact]
    public void Constructor_InvalidPositionId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Domain.Entities.Application(
            candidateId: CandidateId,
            positionId: Guid.Empty));
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesApplication()
    {
        var before = DateTime.UtcNow;

        var application = new Domain.Entities.Application(
            candidateId: CandidateId,
            positionId: PositionId);

        var after = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, application.Id);
        Assert.Equal(CandidateId, application.CandidateId);
        Assert.Equal(PositionId, application.PositionId);
        Assert.Equal(ApplicationStatus.Submitted, application.Status);
        Assert.InRange(application.AppliedAt, before, after);
        Assert.Null(application.DecisionAt);
        Assert.Empty(application.InterviewStages);
    }

    // Testiranje MoveToNextStage() metode
    [Fact]
    public void MoveToNextStage_WhenApplicationIsSubmitted_ChangesStatusToUnderReview()
    {
        var application = CreateValidApplication();

        application.MoveToNextStage(ApplicationStatus.UnderReview);

        Assert.Equal(ApplicationStatus.UnderReview, application.Status);
        Assert.Null(application.DecisionAt);
    }

    [Fact]
    public void MoveToNextStage_WhenApplicationIsUnderReview_ChangesStatusToInterviewScheduled()
    {
        var application = CreateValidApplication();

        application.MoveToNextStage(ApplicationStatus.UnderReview);
        application.MoveToNextStage(ApplicationStatus.InterviewScheduled);

        Assert.Equal(ApplicationStatus.InterviewScheduled, application.Status);
        Assert.Null(application.DecisionAt);
    }

    [Fact]
    public void MoveToNextStage_WhenApplicationIsRejected_SetsStatusAndDecisionAt()
    {
        var application = CreateValidApplication();

        var before = DateTime.UtcNow;

        application.MoveToNextStage(ApplicationStatus.Rejected);

        var after = DateTime.UtcNow;

        Assert.Equal(ApplicationStatus.Rejected, application.Status);
        Assert.NotNull(application.DecisionAt);
        Assert.InRange(application.DecisionAt.Value, before, after);
    }

    [Fact]
    public void MoveToNextStage_WhenApplicationIsAccepted_SetsStatusAndDecisionAt()
    {
        var application = CreateValidApplication();

        var before = DateTime.UtcNow;

        application.MoveToNextStage(ApplicationStatus.Accepted);

        var after = DateTime.UtcNow;

        Assert.Equal(ApplicationStatus.Accepted, application.Status);
        Assert.NotNull(application.DecisionAt);
        Assert.InRange(application.DecisionAt.Value, before, after);
    }

    [Fact]
    public void MoveToNextStage_WhenApplicationIsRejected_ThrowsInvalidOperationException()
    {
        var application = CreateValidApplication();

        application.MoveToNextStage(ApplicationStatus.Rejected);

        Assert.Throws<InvalidOperationException>(() =>
            application.MoveToNextStage(ApplicationStatus.UnderReview));
    }

    [Fact]
    public void MoveToNextStage_WhenApplicationIsAccepted_ThrowsInvalidOperationException()
    {
        var application = CreateValidApplication();

        application.MoveToNextStage(ApplicationStatus.Accepted);

        Assert.Throws<InvalidOperationException>(() =>
            application.MoveToNextStage(ApplicationStatus.UnderReview));
    }

    // Testiranje AddInterviewStage() metode
    [Fact]
    public void AddInterviewStage_WhenApplicationIsOpen_AddsStage()
    {
        var application = CreateValidApplication();

        var stage = application.AddInterviewStage(
            InterviewStageType.Screening);

        Assert.NotNull(stage);
        Assert.Single(application.InterviewStages);
        Assert.Equal(InterviewStageType.Screening, stage.StageType);
        Assert.Equal(application.Id, stage.ApplicationId);
        Assert.Contains(stage, application.InterviewStages);
    }

    [Fact]
    public void AddInterviewStage_WhenApplicationIsRejected_ThrowsInvalidOperationException()
    {
        var application = CreateValidApplication();

        application.MoveToNextStage(ApplicationStatus.Rejected);

        Assert.Throws<InvalidOperationException>(() =>
            application.AddInterviewStage(InterviewStageType.Screening));
    }

    [Fact]
    public void AddInterviewStage_WhenApplicationIsAccepted_ThrowsInvalidOperationException()
    {
        var application = CreateValidApplication();

        application.MoveToNextStage(ApplicationStatus.Accepted);

        Assert.Throws<InvalidOperationException>(() =>
            application.AddInterviewStage(InterviewStageType.Screening));
    }

    [Fact]
    public void AddInterviewStage_WhenSamePendingStageAlreadyExists_ThrowsInvalidOperationException()
    {
        var application = CreateValidApplication();

        application.AddInterviewStage(
            InterviewStageType.TechnicalInterview);

        Assert.Throws<InvalidOperationException>(() =>
            application.AddInterviewStage(
                InterviewStageType.TechnicalInterview));
    }

    [Fact]
    public void AddInterviewStage_WhenDifferentStageIsAdded_AddsStage()
    {
        var application = CreateValidApplication();

        application.AddInterviewStage(
            InterviewStageType.Screening);

        application.AddInterviewStage(
            InterviewStageType.TechnicalInterview);

        Assert.Equal(2, application.InterviewStages.Count);
        Assert.Contains(
            application.InterviewStages,
            x => x.StageType == InterviewStageType.Screening);
        Assert.Contains(
            application.InterviewStages,
            x => x.StageType == InterviewStageType.TechnicalInterview);
    }

    [Fact]
    public void AddInterviewStage_WhenStageWasCompleted_AllowsAddingSameStageAgain()
    {
        var application = CreateValidApplication();

        var firstStage = application.AddInterviewStage(
            InterviewStageType.Screening);

        firstStage.Complete(InterviewOutcome.Passed);

        var secondStage = application.AddInterviewStage(
            InterviewStageType.Screening);

        Assert.Equal(2, application.InterviewStages.Count);
        Assert.Equal(InterviewOutcome.Passed, firstStage.Outcome);
        Assert.Equal(InterviewOutcome.Pending, secondStage.Outcome);
    }
}