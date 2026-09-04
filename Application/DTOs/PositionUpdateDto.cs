using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class PositionUpdateDto
    {
        public string? Title { get; set; } = null;
        public string? Description { get; set; } = null;
        public string? Location { get; set; } = null;
        public int? RequiredExperience { get; set; } = null;
        public EmploymentType? EmploymentType { get; set;} = null;
        public DateTime? Deadline { get; set; } = null;
    }
}