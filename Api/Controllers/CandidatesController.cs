using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/candidates")]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCandidate(CandidateRequestDto request)
        {
            var candidate = await _candidateService.CreateAsync(request);
            return CreatedAtAction(nameof(GetCandidateById), new { id = candidate.Id }, candidate);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCandidateById(Guid id)
        {
            var candidate = await _candidateService.GetByIdAsync(id);
            if (candidate == null) return NotFound();
            return Ok(candidate);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> GetAllCandidates()
        {
            var candidates = await _candidateService.GetAllAsync();
            return Ok(candidates);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> UpdateCandidate(Guid id, CandidateUpdateDto updateDto)
        {
            var candidate = await _candidateService.UpdateAsync(id, updateDto);
            if (candidate == null) return NotFound();
            return Ok(candidate);
        }

        [HttpPost("{id}/resume")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> UploadResume(Guid id, UploadResumeRequestDto request)
        {
            await _candidateService.UploadResumeAsync(id, request.ResumeUrl);
            return Ok();
        }

        [HttpPost("{id}/skills")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> AddSkill(Guid id, AddSkillRequestDto request)
        {
            await _candidateService.AddSkillAsync(id, request.SkillName);
            return Ok();
        }

        [HttpDelete("{id}/skills/{skillName}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> RemoveSkill(Guid id, [FromRoute] string skillName)
        {
            await _candidateService.RemoveSkillAsync(id, skillName);
            return Ok();
        }
    }
}
