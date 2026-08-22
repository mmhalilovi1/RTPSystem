using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Domain.Entities
{
    public class Position
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }
        public EmploymentType EmploymentType { get; private set; }
        private readonly List<string> requiredSkills = new();
        public IReadOnlyCollection<string> RequiredSkills => requiredSkills;
        public int RequiredExperience { get; private set; }
        public PositionStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public DateTime Deadline {  get; private set; }

        //public User CreatedBy { get; private set; }

        private Position() { }

        public Position(
            string title, string description, string location, EmploymentType emplType, 
            List<string> skills, int experience, DateTime deadline)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Naslov pozicije je obavezan.", nameof(title));

            if (experience < 0)
                throw new ArgumentException("Traženo iskustvo ne može biti negativno.", nameof(experience));

            if (deadline <= DateTime.UtcNow)
                throw new ArgumentException("Rok mora biti u budućnosti.", nameof(deadline));

            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Location = location;
            EmploymentType = emplType;
            requiredSkills = skills;
            RequiredExperience = experience;
            Status = PositionStatus.Draft;
            CreatedAt = DateTime.UtcNow;
            Deadline = deadline;
        }

        public void Publish()
        {
            if (Status != PositionStatus.Draft)
                throw new ArgumentException("Pozicija je već otvorena");

            Status = PositionStatus.Open;
        }

        public void Close()
        {
            if (Status != PositionStatus.Open)
                throw new ArgumentException("Pozicija nije otvorena");
            
            Status = PositionStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != PositionStatus.Closed)
                throw new ArgumentException("Ne možete arhivirati aktivne pozicije");

            Status = PositionStatus.Archived;
        }

        public void UpdateDetails(
           string? title = null, string? description = null, string? location = null,
           int? requiredExperience = null, EmploymentType? employmentType = null,
           DateTime? deadline = null)
        {
            if (Status == PositionStatus.Closed || Status == PositionStatus.Archived)
                throw new InvalidOperationException("Detalji zatvorene ili arhivirane pozicije se ne mogu mijenjati.");

            if (title != null)
            {
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("Naslov ne može biti prazan.", nameof(title));
                Title = title;
            }

            if (description != null) Description = description;
            if (location != null) Location = location;

            if (requiredExperience.HasValue)
            {
                if (requiredExperience.Value < 0)
                    throw new ArgumentException("Iskustvo ne može biti negativno.", nameof(requiredExperience));
                RequiredExperience = requiredExperience.Value;
            }

            if (employmentType.HasValue) EmploymentType = employmentType.Value;

            if (deadline.HasValue)
            {
                if (deadline.Value <= DateTime.UtcNow)
                    throw new ArgumentException("Rok mora biti u budućnosti.", nameof(deadline));
                Deadline = deadline.Value;
            }
        }

        public bool isOpen()
        {
            return Status == PositionStatus.Open && Deadline > DateTime.UtcNow;
        }

        public void AddRequiredSkill(string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
                throw new ArgumentException("Vještina ne može biti prazna.", nameof(skill));

            if (requiredSkills.Contains(skill))
                throw new InvalidOperationException("Vještina već postoji.");

            requiredSkills.Add(skill);
        }

        public void RemoveRequiredSkill(string skill)
        {
            if (!requiredSkills.Contains(skill))
                throw new InvalidOperationException("Skill ne postoji");

            requiredSkills.Remove(skill);
        }
    }
}
