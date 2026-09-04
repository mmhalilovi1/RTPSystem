using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class PositionRequestDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }
        [Required, MaxLength(4000)]
        public string Description { get; set; }
        [Required, MaxLength(200)]
        public string Location { get; set; }
        [Required]
        public EmploymentType EmploymentType { get; set; }
        [Required]
        [Range(0, 40)]
        public int RequiredExperience { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        [Required]
        public List<string> RequiredSkills { get; set; }
    }
}
