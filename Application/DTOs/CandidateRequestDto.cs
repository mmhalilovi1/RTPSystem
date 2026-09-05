using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class CandidateRequestDto
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; }
        [Required, MaxLength(50)]
        public string PhoneNumber { get; set; }
        [Required, Range(0, 40)]
        public int YearsOfExperience { get; set; }
        [Required]
        public List<string> Skills { get; set; }
    }
}
