using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Text;

namespace Application.DTOs
{
    public class CandidateResponseDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required, MaxLength(200)]
        public string FullName { get; set; }
        [Required, MaxLength(50)]
        public string PhoneNumber { get; set; }
        [Required, Range(0, 40)]
        public int YearsOfExperience { get; set; }
        [Required]
        public List<string> Skills { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

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
                CreatedAt = candidate.CreatedAt
            };
        }
    }
}
