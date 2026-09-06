using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class SubmitApplicationRequestDto
    {
        [Required]
        public Guid PositionId { get; set; }   
    }
}
