using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class InterviewStage
    {
        public Guid Id { get; private set; }
        public Guid ApplicationId { get; private set; }
        public InterviewStageType StageType { get; private set; }
        public InterviewOutcome Outcome { get; private set; }
        public DateTime? ScheduledAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public string? Notes { get; private set; }

        private InterviewStage() { }

        public InterviewStage(Guid applicationId, InterviewStageType stageType)
        {
            if (applicationId == Guid.Empty)
                throw new ArgumentException("ApplicationId je obavezan.", nameof(applicationId));

            Id = Guid.NewGuid();
            ApplicationId = applicationId;
            StageType = stageType;
            Outcome = InterviewOutcome.Pending;
        }

        public void Schedule(DateTime scheduledAt)
        {
            if (Outcome != InterviewOutcome.Pending)
                throw new InvalidOperationException("Faza je već završena, ne može se ponovo zakazati.");

            if (scheduledAt <= DateTime.UtcNow)
                throw new ArgumentException("Termin mora biti u budućnosti.", nameof(scheduledAt));

            ScheduledAt = scheduledAt;
        }

        public void Complete(InterviewOutcome outcome, string? notes = null)
        {
            if (outcome == InterviewOutcome.Pending)
                throw new ArgumentException("Ishod mora biti Passed ili Failed.", nameof(outcome));

            if (Outcome != InterviewOutcome.Pending)
                throw new InvalidOperationException("Faza je već završena.");

            Outcome = outcome;
            Notes = notes;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
