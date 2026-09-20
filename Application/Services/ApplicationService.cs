using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationService(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApplicationResponseDto> SubmitApplicationAsync(SubmitApplicationRequestDto request)
        {
            var userId = _currentUserService.UserId;

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (candidate == null)
                throw new InvalidOperationException("Morate imati kandidatski profil prije apliciranja.");

            var position = await _context.Positions
                .FirstOrDefaultAsync(p => p.Id == request.PositionId);
            if (position == null)
                throw new KeyNotFoundException("Pozicija nije pronađena.");

            var alreadyApplied = await _context.Applications
                .AnyAsync(a => a.CandidateId == candidate.Id && a.PositionId == position.Id);

            ApplicationEligibilityChecker.EnsureCanApply(candidate, position, alreadyApplied);

            var application = new Domain.Entities.Application(candidate.Id, position.Id);

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return ApplicationResponseDto.FromEntity(application);
        }     

        public async Task<ApplicationResponseDto?> GetByIdAsync(Guid applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.InterviewStages)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null) return null;

            var isPrivileged = _currentUserService.IsInRole("Admin") || _currentUserService.IsInRole("Recruiter");

            if (!isPrivileged)
            {
                var candidate = await _context.Candidates
                    .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId);

                if (candidate == null || application.CandidateId != candidate.Id)
                    throw new UnauthorizedAccessException("Nemate dozvolu da vidite ovu prijavu.");
            }

            return ApplicationResponseDto.FromEntity(application);
        }

        public async Task<List<ApplicationResponseDto>> GetMyApplicationsAsync()
        {
            var userId = _currentUserService.UserId;
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
                throw new InvalidOperationException("Morate imati kandidatski profil da biste vidjeli svoje prijave.");

            var applications = await _context.Applications
                .Include(a => a.InterviewStages)
                .Where(a => a.CandidateId == candidate.Id)
                .ToListAsync();

            var positionIds = applications.Select(a => a.PositionId).Distinct().ToList();
            var positionTitles = await _context.Positions
                .Where(p => positionIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Title);

            return applications.Select(a =>
            {
                var dto = ApplicationResponseDto.FromEntity(a);
                dto.PositionTitle = positionTitles.GetValueOrDefault(a.PositionId);
                return dto;
            }).ToList();
        }

        public async Task<List<ApplicationResponseDto>> GetByPositionAsync(Guid positionId)
        {
            var applications = await _context.Applications
                .Include(a => a.InterviewStages)
                .Where(a => a.PositionId == positionId)
                .ToListAsync();

            var candidateIds = applications.Select(a => a.CandidateId).Distinct().ToList();
            var candidateNames = await _context.Candidates
                .Where(c => candidateIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.FullName);

            return applications.Select(a =>
            {
                var dto = ApplicationResponseDto.FromEntity(a);
                dto.CandidateFullName = candidateNames.GetValueOrDefault(a.CandidateId);
                return dto;
            }).ToList();
        }

        public async Task<InterviewStageResponseDto> AddInterviewStageAsync(Guid applicationId, InterviewStageType stageType)
        {
            var application = await _context.Applications
                .Include(a => a.InterviewStages)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                throw new KeyNotFoundException("Prijava nije pronađena.");

            var stage = application.AddInterviewStage(stageType);
            _context.InterviewStages.Add(stage);

            await _context.SaveChangesAsync();
            return InterviewStageResponseDto.FromEntity(stage);
        }

        public async Task<InterviewStageResponseDto> ScheduleInterviewStageAsync(Guid applicationId, Guid interviewStageId, DateTime scheduledAt)
        {
            var application = await _context.Applications
                .Include(a => a.InterviewStages)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                throw new KeyNotFoundException("Prijava nije pronađena.");

            var interviewStage = application.InterviewStages
                .FirstOrDefault(s => s.Id == interviewStageId);

            if (interviewStage == null)
                throw new KeyNotFoundException("Intervju faza nije pronađena.");

            interviewStage.Schedule(scheduledAt);

            await _context.SaveChangesAsync();
            return InterviewStageResponseDto.FromEntity(interviewStage);
        }

        public async Task<InterviewStageResponseDto> CompleteInterviewStageAsync(Guid applicationId, Guid interviewStageId, InterviewOutcome outcome, string? notes = null)
        {
            var application = await _context.Applications
                .Include(a => a.InterviewStages)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                throw new KeyNotFoundException("Prijava nije pronađena.");

            var interviewStage = application.InterviewStages
                .FirstOrDefault(s => s.Id == interviewStageId);

            if (interviewStage == null)
                throw new KeyNotFoundException("Intervju faza nije pronađena.");

            interviewStage.Complete(outcome, notes);

            if (outcome == InterviewOutcome.Failed)
            {
                application.MoveToNextStage(ApplicationStatus.Rejected);
            }
            else if (interviewStage.StageType == InterviewStageType.FinalDecision)
            {
                application.MoveToNextStage(ApplicationStatus.Accepted);
            }
            else
            {
                application.MoveToNextStage(ApplicationStatus.InterviewScheduled);
            }

            await _context.SaveChangesAsync();
            return InterviewStageResponseDto.FromEntity(interviewStage);
        }

        public async Task<FeedbackResponseDto> AddFeedbackAsync(Guid applicationId, Guid stageId, FeedbackRequestDto request)
        {
            var application = await _context.Applications
                .Include(a => a.InterviewStages)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                throw new KeyNotFoundException("Prijava nije pronađena");

            var interviewStage = application.InterviewStages.FirstOrDefault(s => s.Id == stageId);

            if (interviewStage == null)
                throw new KeyNotFoundException("Intervju faza nije pronađena.");

            if (interviewStage.Outcome == InterviewOutcome.Pending)
                throw new InvalidOperationException("Ne možete ostaviti feedback na fazu koja nije završena.");

            var feedback = new Feedback(
                stageId,
                _currentUserService.UserId,
                request.Rating,
                request.Comments
            );

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return FeedbackResponseDto.FromEntity(feedback);
        }

        public async Task<List<FeedbackResponseDto>> GetFeedbackForStageAsync(Guid applicationId, Guid stageId)
        {
            var application = await _context.Applications
                .Include(a => a.InterviewStages)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                throw new KeyNotFoundException("Prijava nije pronađena");

            var interviewStage = application.InterviewStages.FirstOrDefault(s => s.Id == stageId);

            if (interviewStage == null)
                throw new KeyNotFoundException("Intervju faza nije pronađena.");

            var feedbacks = await _context.Feedbacks
                .Where(f => f.InterviewStageId == interviewStage.Id)
                .ToListAsync();

            return feedbacks.Select(FeedbackResponseDto.FromEntity).ToList();
        }
    }
}