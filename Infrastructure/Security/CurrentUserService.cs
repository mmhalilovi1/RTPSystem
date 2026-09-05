using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var userIdClaim = user?.FindFirst(JwtRegisteredClaimNames.Sub);

                if (userIdClaim == null)
                    throw new UnauthorizedAccessException("Korisnik nije autentifikovan.");

                if (!Guid.TryParse(userIdClaim.Value, out var userId))
                    throw new UnauthorizedAccessException("Neispravan UserId.");

                return userId;
            }
        }

        public bool IsInRole(string role)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new UnauthorizedAccessException("Korisnik nije autentifikovan.");
            return user.IsInRole(role);
        }
    }
}
