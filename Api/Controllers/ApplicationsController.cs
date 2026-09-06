using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> SubmitApplication(SubmitApplicationRequestDto request)
        {
            var response = await _applicationService.SubmitApplicationAsync(request);
            return CreatedAtAction(nameof(GetApplicationById), new { id = response.Id }, response);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetApplicationById(Guid id)
        {
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null) return NotFound();
            return Ok(application);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetMyApplications()
        {
            var applications = await _applicationService.GetMyApplicationsAsync();
            return Ok(applications);
        }

        [HttpGet("by-position/{positionId:guid}")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> GetApplicationsByPosition(Guid positionId)
        {
            var applications = await _applicationService.GetByPositionAsync(positionId);
            return Ok(applications);
        }

        [HttpPost("{id:guid}/interview-stages")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> AddInterviewStage(Guid id, InterviewStageRequestDto request)
        {
            var result = await _applicationService.AddInterviewStageAsync(id, request.StageType);
            return CreatedAtAction(nameof(GetApplicationById), new { id }, result);
        }

        [HttpPatch("{id:guid}/interview-stages/{stageId:guid}/schedule")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> ScheduleInterviewStage(Guid id, Guid stageId, ScheduleInterviewStageRequestDto request)
        {
            var result = await _applicationService.ScheduleInterviewStageAsync(id, stageId, request.ScheduledAt);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/interview-stages/{stageId:guid}/complete")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> CompleteInterviewStage(Guid id, Guid stageId, CompleteInterviewStageRequestDto request)
        {
            var result = await _applicationService.CompleteInterviewStageAsync(id, stageId, request.Outcome, request.Notes);
            return Ok(result);
        }
    }
}
