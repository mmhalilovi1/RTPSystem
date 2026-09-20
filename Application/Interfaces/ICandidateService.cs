using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICandidateService
    {
        Task<CandidateResponseDto> CreateAsync(CandidateRequestDto request); 
        Task<CandidateResponseDto?> GetByIdAsync(Guid id);  
        Task<List<CandidateResponseDto>> GetAllAsync(); 
        Task<CandidateResponseDto?> UpdateAsync(Guid id, CandidateUpdateDto updateDto); 
        Task UploadResumeAsync(Guid id, string resumeUrl); 
        Task AddSkillAsync(Guid id, string skill);
        Task RemoveSkillAsync(Guid id, string skill);
        Task<CandidateResponseDto?> GetMyProfileAsync();
    }
}
