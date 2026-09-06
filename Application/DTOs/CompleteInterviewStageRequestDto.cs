using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CompleteInterviewStageRequestDto
    {
        [Required]
        public InterviewOutcome Outcome { get; set; }
        public string? Notes { get; set; } = null;
    }
}
