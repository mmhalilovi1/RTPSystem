using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Text;

namespace Application.DTOs
{
    public class CandidateResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public List<string> Skills { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ResumeUrl { get; set; }

        public static CandidateResponseDto FromEntity(Domain.Entities.Candidate candidate)
        {
            return new CandidateResponseDto
            {
                Id = candidate.Id,
                UserId = candidate.UserId,
                FullName = candidate.FullName,
                PhoneNumber = candidate.PhoneNumber,
                YearsOfExperience = candidate.YearsOfExperience,
                Skills = candidate.Skills?.Select(s => s.Name).ToList() ?? new List<string>(),
                CreatedAt = candidate.CreatedAt,
                ResumeUrl = candidate.ResumeUrl
            };
        }
    }
}
