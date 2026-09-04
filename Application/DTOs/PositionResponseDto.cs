using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class PositionResponseDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; }
        [Required, MaxLength(4000)]
        public string Description { get; set; }
        [Required, MaxLength(200)]
        public string Location { get; set; }
        [Required, MaxLength(20)]
        public EmploymentType EmploymentType { get; set; }
        [Required]
        [Range(0, 40)]
        public int RequiredExperience { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        [Required]
        public List<string> RequiredSkills { get; set; }
        [Required, MaxLength(20)]
        public PositionStatus Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public static PositionResponseDto FromEntity(Position position)
        {
            return new PositionResponseDto
            {
                Id = position.Id,
                Title = position.Title,
                Description = position.Description,
                Location = position.Location,
                EmploymentType = position.EmploymentType,
                RequiredExperience = position.RequiredExperience,
                Deadline = position.Deadline,
                RequiredSkills = position.RequiredSkills?.Select(s => s.Name).ToList() ?? new List<string>(),
                Status = position.Status,
                CreatedAt = position.CreatedAt
            };
        }
    }
}
