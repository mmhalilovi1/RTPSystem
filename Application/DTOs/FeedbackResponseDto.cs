namespace Application.DTOs
{
    public class FeedbackResponseDto
    {
        public Guid Id { get; set; }
        public Guid InterviewStageId { get; set; }
        public Guid AuthorUserId { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }

        public static FeedbackResponseDto FromEntity(Domain.Entities.Feedback feedback)
        {
            return new FeedbackResponseDto
            {
                Id = feedback.Id,
                InterviewStageId = feedback.InterviewStageId,
                AuthorUserId = feedback.AuthorUserId,
                Rating = feedback.Rating,
                Comments = feedback.Comments,
                CreatedAt = feedback.CreatedAt
            };
        }
    }
}
