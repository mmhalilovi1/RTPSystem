using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Candidate
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }   
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? ResumeUrl { get; private set; }
        public int YearsOfExperience { get; private set; }
        private readonly List<Skill> skills = new();
        public IReadOnlyCollection<Skill> Skills => skills.AsReadOnly();
        public DateTime CreatedAt { get; private set; }

        private Candidate() { }

        public Candidate(Guid userId, string fullName, string phoneNumber, int yearsOfExperience, List<Skill> skillss)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId je obavezan.", nameof(userId));

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Ime i prezime su obavezni.", nameof(fullName));

            if (yearsOfExperience < 0)
                throw new ArgumentException("Iskustvo ne može biti negativno.", nameof(yearsOfExperience));

            Id = Guid.NewGuid();
            UserId = userId;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            YearsOfExperience = yearsOfExperience;
            skills = skillss;
            CreatedAt = DateTime.UtcNow;
        }

        public bool CanApply() => !string.IsNullOrWhiteSpace(ResumeUrl);

        public void UploadResume(string resumeUrl)
        {
            if (string.IsNullOrWhiteSpace(resumeUrl))
                throw new ArgumentException("URL CV-a nije validan.", nameof(resumeUrl));

            ResumeUrl = resumeUrl;
        }

        public void UpdateProfile(string? fullName = null, string? phoneNumber = null, int? yearsOfExperience = null)
        {
            if (fullName != null)
            {
                if (string.IsNullOrWhiteSpace(fullName))
                    throw new ArgumentException("Ime i prezime ne mogu biti prazni.", nameof(fullName));
                FullName = fullName;
            }

            if (phoneNumber != null) PhoneNumber = phoneNumber;

            if (yearsOfExperience.HasValue)
            {
                if (yearsOfExperience.Value < 0)
                    throw new ArgumentException("Iskustvo ne može biti negativno.", nameof(yearsOfExperience));
                YearsOfExperience = yearsOfExperience.Value;
            }
        }

        public void AddSkill(Skill skill)
        {
            if (skills.Any(s => s.Name == skill.Name))
                throw new InvalidOperationException("Vještina je već dodana.");

            skills.Add(skill);
        }

        public void RemoveSkill(Skill skill)
        {
            var existingSkill = skills.FirstOrDefault(s => s.Name == skill.Name);

            if (existingSkill == null)
                throw new InvalidOperationException("Vještina ne postoji.");

            skills.Remove(existingSkill);
        }
    }
}
