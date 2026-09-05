using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class PositionUpdateDto
    {
        [MaxLength(200)]
        public string? Title { get; set; } = null;
        [MaxLength(4000)]
        public string? Description { get; set; } = null;
        [MaxLength(200)]
        public string? Location { get; set; } = null;
        [Range(0, 40)]
        public int? RequiredExperience { get; set; } = null;
        public EmploymentType? EmploymentType { get; set;} = null;
        public DateTime? Deadline { get; set; } = null;
    }
}