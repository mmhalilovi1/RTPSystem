using Application.Interfaces;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CandidateService(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<CandidateResponseDto> CreateAsync(CandidateRequestDto request)
        {
            var userId = _currentUserService.UserId;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            var alreadyExists = await _context.Candidates.AnyAsync(c => c.UserId == userId);
            if (alreadyExists)
                throw new InvalidOperationException("Kandidatski profil već postoji za ovog korisnika.");

            var skills = new List<Skill>();
            foreach (var skillName in request.Skills)
            {
                var existing = await _context.Skills.FirstOrDefaultAsync(s => s.Name == skillName);
                skills.Add(existing ?? new Skill(skillName));
            }

            var candidate = new Candidate(userId, request.FullName, request.PhoneNumber, request.YearsOfExperience, skills);

            user.BecomeCandidate();

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return CandidateResponseDto.FromEntity(candidate);
        }

        public async Task<CandidateResponseDto?> GetByIdAsync(Guid id)
        {
            var candidate = await _context.Candidates.Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == id);
            if (candidate == null) return null;

            var isOwner = candidate.UserId == _currentUserService.UserId;
            var isPrivileged = _currentUserService.IsInRole("Admin") || _currentUserService.IsInRole("Recruiter");

            if (!isOwner && !isPrivileged)
                throw new UnauthorizedAccessException("Nemate dozvolu da vidite ovaj profil.");

            return CandidateResponseDto.FromEntity(candidate);
        }

        public async Task<List<CandidateResponseDto>> GetAllAsync()
        {
            var candidates = await _context.Candidates
                .Include(c => c.Skills)
                .ToListAsync();

            return candidates.Select(CandidateResponseDto.FromEntity).ToList();
        }

        public async Task<CandidateResponseDto?> UpdateAsync(Guid id, CandidateUpdateDto updateDto)
        {
            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null) return null;

            if (candidate.UserId != _currentUserService.UserId)
                throw new UnauthorizedAccessException("Nemate dozvolu da ažurirate ovaj profil kandidata.");
            
            candidate.UpdateProfile(updateDto.FullName, updateDto.PhoneNumber, updateDto.YearsOfExperience);

            await _context.SaveChangesAsync();
            return CandidateResponseDto.FromEntity(candidate);
        }

        public async Task UploadResumeAsync(Guid id, string resumeUrl)
        {
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null) throw new KeyNotFoundException("Kandidat nije pronađen.");
            if (candidate.UserId != _currentUserService.UserId)
                throw new UnauthorizedAccessException("Nemate dozvolu da ažurirate ovaj profil kandidata.");

            candidate.UploadResume(resumeUrl);
            await _context.SaveChangesAsync();
        }

        public async Task AddSkillAsync(Guid id, string skill)
        {
            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null) throw new KeyNotFoundException("Kandidat nije pronađen.");
            if (candidate.UserId != _currentUserService.UserId)
                throw new UnauthorizedAccessException("Nemate dozvolu da ažurirate ovaj profil kandidata.");

            var existingSkill = await _context.Skills.FirstOrDefaultAsync(s => s.Name == skill);

            Skill skillToAdd;
            if (existingSkill != null)
            {
                skillToAdd = existingSkill;
            }
            else
            {
                skillToAdd = new Skill(skill);
                _context.Skills.Add(skillToAdd);
            }

            candidate.AddSkill(skillToAdd);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveSkillAsync(Guid id, string skill)
        {
            var candidate = await _context.Candidates
                .Include (c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null) throw new KeyNotFoundException("Kandidat nije pronađen.");
            if (candidate.UserId != _currentUserService.UserId)
                throw new UnauthorizedAccessException("Nemate dozvolu da ažurirate ovaj profil kandidata.");

            var skillToRemove = candidate.Skills.FirstOrDefault(s => s.Name == skill);
            if (skillToRemove == null) throw new KeyNotFoundException("Veština nije pronađena.");

            candidate.RemoveSkill(skillToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<CandidateResponseDto?> GetMyProfileAsync()
        {
            var userId = _currentUserService.UserId;

            var candidate = await _context.Candidates
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return candidate == null ? null : CandidateResponseDto.FromEntity(candidate);
        }
    }
}
