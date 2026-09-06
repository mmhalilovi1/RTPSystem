using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ScheduleInterviewStageRequestDto
    {
        [Required]
        public DateTime ScheduledAt { get; set; }
    }
}
