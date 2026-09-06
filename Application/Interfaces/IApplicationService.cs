using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IApplicationService
    {
        Task<ApplicationResponseDto> SubmitApplicationAsync(SubmitApplicationRequestDto request);
        Task<ApplicationResponseDto?> GetByIdAsync(Guid applicationId);
        Task<List<ApplicationResponseDto>> GetMyApplicationsAsync();
        Task<List<ApplicationResponseDto>> GetByPositionAsync(Guid positionId);
        Task<InterviewStageResponseDto> AddInterviewStageAsync(Guid applicationId, InterviewStageType stageType);
        Task<InterviewStageResponseDto> ScheduleInterviewStageAsync(Guid applicationId, Guid interviewStageId, DateTime scheduledAt);
        Task<InterviewStageResponseDto> CompleteInterviewStageAsync(Guid applicationId, Guid interviewStageId, InterviewOutcome outcome, string? notes = null);
    }
}
