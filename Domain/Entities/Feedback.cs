using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Feedback
    {
        public Guid Id { get; private set; }
        public Guid InterviewStageId { get; private set; }
        public Guid AuthorUserId { get; private set; }
        public int Rating { get; private set; } 
        public string Comments { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Feedback() { }

        public Feedback(Guid interviewStageId, Guid authorUserId, int rating, string comments)
        {
            if (interviewStageId == Guid.Empty)
                throw new ArgumentException("InterviewStageId je obavezan.", nameof(interviewStageId));

            if (authorUserId == Guid.Empty)
                throw new ArgumentException("AuthorUserId je obavezan.", nameof(authorUserId));

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Ocjena mora biti između 1 i 5.", nameof(rating));

            if (string.IsNullOrWhiteSpace(comments))
                throw new ArgumentException("Komentar je obavezan.", nameof(comments));

            Id = Guid.NewGuid();
            InterviewStageId = interviewStageId;
            AuthorUserId = authorUserId;
            Rating = rating;
            Comments = comments;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
