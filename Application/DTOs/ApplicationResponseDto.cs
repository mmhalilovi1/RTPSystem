using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class ApplicationResponseDto
    {
        
        public Guid Id { get; set; }
        
        public Guid PositionId { get; set; }
        
        public Guid CandidateId { get; set; }
        
        public ApplicationStatus Status { get; set; }
        
        public DateTime AppliedAt { get; set; }
        public DateTime? DecisionAt { get; set; }
        public string? PositionTitle { get; set; }
        public string? CandidateFullName { get; set; }
        public string? CandidateResumeUrl { get; set; }

        public List<InterviewStageResponseDto> InterviewStages { get; set; } = new List<InterviewStageResponseDto>();
    
        public static ApplicationResponseDto FromEntity(Domain.Entities.Application application)
        {
            return new ApplicationResponseDto
            {
                Id = application.Id,
                PositionId = application.PositionId,
                CandidateId = application.CandidateId,
                Status = application.Status,
                AppliedAt = application.AppliedAt,
                DecisionAt = application.DecisionAt,
                InterviewStages = application.InterviewStages?.Select(InterviewStageResponseDto.FromEntity).ToList() ?? new List<InterviewStageResponseDto>()
            };
        }
    }
}