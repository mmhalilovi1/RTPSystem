using Domain.Entities;

public class FeedbackTests
{
    private static readonly Guid InterviewStageId = Guid.NewGuid();
    private static readonly Guid AuthorUserId = Guid.NewGuid();

    // Testiranje konstruktora
    [Fact]
    public void Constructor_InvalidInterviewStageId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Feedback(
            interviewStageId: Guid.Empty,
            authorUserId: AuthorUserId,
            rating: 5,
            comments: "Odličan kandidat."));
    }

    [Fact]
    public void Constructor_InvalidAuthorUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: Guid.Empty,
            rating: 5,
            comments: "Odličan kandidat."));
    }

    [Fact]
    public void Constructor_RatingBelowOne_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: AuthorUserId,
            rating: 0,
            comments: "Komentar."));
    }

    [Fact]
    public void Constructor_RatingAboveFive_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: AuthorUserId,
            rating: 6,
            comments: "Komentar."));
    }

    [Fact]
    public void Constructor_EmptyComments_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: AuthorUserId,
            rating: 5,
            comments: ""));
    }

    [Fact]
    public void Constructor_WhitespaceComments_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: AuthorUserId,
            rating: 5,
            comments: "   "));
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesFeedback()
    {
        var before = DateTime.UtcNow;

        var feedback = new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: AuthorUserId,
            rating: 5,
            comments: "Odličan kandidat.");

        var after = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, feedback.Id);
        Assert.Equal(InterviewStageId, feedback.InterviewStageId);
        Assert.Equal(AuthorUserId, feedback.AuthorUserId);
        Assert.Equal(5, feedback.Rating);
        Assert.Equal("Odličan kandidat.", feedback.Comments);
        Assert.InRange(feedback.CreatedAt, before, after);
    }

    [Fact]
    public void Constructor_RatingOne_IsValid()
    {
        var feedback = new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: AuthorUserId,
            rating: 1,
            comments: "Loš kandidat.");

        Assert.Equal(1, feedback.Rating);
    }

    [Fact]
    public void Constructor_RatingFive_IsValid()
    {
        var feedback = new Feedback(
            interviewStageId: InterviewStageId,
            authorUserId: AuthorUserId,
            rating: 5,
            comments: "Odličan kandidat.");

        Assert.Equal(5, feedback.Rating);
    }
}