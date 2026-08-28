using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public string TokenHash { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public Guid UserId { get; private set; }

        public RefreshToken() { }
        public RefreshToken(Guid userId, string tokenHash)
        {
            Id = Guid.NewGuid();
            //Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            TokenHash = tokenHash;
            ExpiresAt = DateTime.UtcNow.AddDays(10);
            UserId = userId;
        }

        public void Revoke()
        {
            if (ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Refresh token je istekao");

            RevokedAt = DateTime.UtcNow;
        }
        
        public bool IsExpired()
        {
            return ExpiresAt < DateTime.UtcNow;
        }
    }
}
