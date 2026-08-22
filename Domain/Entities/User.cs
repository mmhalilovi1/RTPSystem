using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private User() { }

        public User(string email, string passwordHash, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ArgumentException("Email nije validan.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Lozinka je obavezna.", nameof(passwordHash));

            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // hashovanje se ne radi ovdje jer entitet prima već hashovanu lozinku
        // Hash algoritam je briga infrastrukture
        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Nova lozinka je obavezna.", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Korisnik je već deaktiviran.");

            IsActive = false;
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new InvalidOperationException("Korisnik je već aktivan.");

            IsActive = true;
        }
    }
}
