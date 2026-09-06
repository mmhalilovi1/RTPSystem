using Application.DTOs;
using Application.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/positions")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionsController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> Create(PositionRequestDto request)
        {
            var result = await _positionService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _positionService.GetByIdAsync(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("{id:guid}/open")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> GetByIdOpen(Guid id)
        {
            var result = await _positionService.GetbyIdOpenAsync(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> GetAll([FromQuery] PositionFilterDto filter)
        {
            var result = await _positionService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("open")]
        public async Task<IActionResult> GetAllOpen([FromQuery] PositionFilterDto filter)
        {
            var result = await _positionService.GetAllOpenAsync(filter);
            return Ok(result);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> Update(Guid id, PositionUpdateDto updateDto)
        { 
            var result = await _positionService.UpdateAsync(id, updateDto);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> Delete(Guid id)
        {        
            await _positionService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id:guid}/publish")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> Publish(Guid id)
        {            
            await _positionService.PublishAsync(id);                     
            return Ok();
        }

        [HttpPost("{id:guid}/close")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> Close(Guid id)
        {
            await _positionService.CloseAsync(id);
            return Ok();
        }

        [HttpPost("{id:guid}/archive")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> Archive(Guid id)
        {
            await _positionService.ArchiveAsync(id);            
            return Ok();
        }
    }
}
