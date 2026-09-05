using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class UploadResumeRequestDto
    {
        [Required]
        public string ResumeUrl { get; set; }
    }
}
