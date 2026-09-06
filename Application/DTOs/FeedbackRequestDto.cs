using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class FeedbackRequestDto
    {
        [Required, Range(1, 5)]
        public int Rating { get; set; }
        [Required, MaxLength(4000)]
        public string Comments { get; set; }
    }
}
