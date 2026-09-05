using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class AddSkillRequestDto
    {
        [Required]
        public string SkillName { get; set; }
    }
}
