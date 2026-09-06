using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Application
    {
        public Guid Id { get; private set; }
        public Guid CandidateId { get; private set; }
        public Guid PositionId { get; private set; }
        public ApplicationStatus Status { get; private set; }
        public DateTime AppliedAt { get; private set; }
        public DateTime? DecisionAt { get; private set; }
        private readonly List<InterviewStage> interviewStages = new();
        public IReadOnlyCollection<InterviewStage> InterviewStages => interviewStages;
       
        private Application() { }

        public Application(Guid candidateId, Guid positionId)
        {
            if (candidateId == Guid.Empty)
                throw new ArgumentException("CandidateId ne može biti prazan.", nameof(candidateId));

            if (positionId == Guid.Empty)
                throw new ArgumentException("PositionId ne može biti prazan.", nameof(positionId));

            Id = Guid.NewGuid();
            CandidateId = candidateId;
            PositionId = positionId;
            Status = ApplicationStatus.Submitted;
            AppliedAt = DateTime.UtcNow;
        }

        public void MoveToNextStage(ApplicationStatus nextStatus)
        {
            if (Status == ApplicationStatus.Rejected || Status == ApplicationStatus.Accepted)
                throw new InvalidOperationException(
                    "Ne može se mijenjati status aplikacije koja je već zatvorena.");

            Status = nextStatus;

            if (nextStatus == ApplicationStatus.Rejected || nextStatus == ApplicationStatus.Accepted)
                DecisionAt = DateTime.UtcNow;
        }

        public InterviewStage AddInterviewStage(InterviewStageType stageType)
        {
            if (Status == ApplicationStatus.Rejected || Status == ApplicationStatus.Accepted)
                throw new InvalidOperationException("Ne može se dodati faza na zatvorenu aplikaciju.");

            if (interviewStages.Any(s => s.StageType == stageType && s.Outcome == InterviewOutcome.Pending))
                throw new InvalidOperationException("Ova faza je već u toku.");

            var stage = new InterviewStage(Id, stageType);
            interviewStages.Add(stage);
            return stage;
        }
    }
}
