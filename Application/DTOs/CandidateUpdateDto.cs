using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class CandidateUpdateDto
    {
        [MaxLength(200)]
        public string? FullName { get; set; } = null;
        [MaxLength(50)]
        public string? PhoneNumber { get; set; } = null;
        [Range(0, 40)]
        public int? YearsOfExperience { get; set; } = null;
        public List<string> Skills { get; set; } = null;
    }
}
