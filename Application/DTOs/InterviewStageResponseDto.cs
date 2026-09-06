using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class InterviewStageResponseDto
    {
        public Guid Id { get; set; }
        public InterviewStageType StageType { get; set; }
        public InterviewOutcome Outcome { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Notes { get; set; }

        public static InterviewStageResponseDto FromEntity(Domain.Entities.InterviewStage interviewStage)
        {
            return new InterviewStageResponseDto
            {
                Id = interviewStage.Id,
                StageType = interviewStage.StageType,
                Outcome = interviewStage.Outcome,
                ScheduledAt = interviewStage.ScheduledAt,
                CompletedAt = interviewStage.CompletedAt,
                Notes = interviewStage.Notes
            };
        }
    }
}
