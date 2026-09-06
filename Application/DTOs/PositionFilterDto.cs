using Domain.Enums;

namespace Application.DTOs
{
    public class PositionFilterDto
    {
        public string? Location { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public int? MinRequiredExperience { get; set; }
    }
}