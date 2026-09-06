using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class InterviewStageRequestDto
    {
        [Required]
        public InterviewStageType StageType { get; set; }
    }
}
