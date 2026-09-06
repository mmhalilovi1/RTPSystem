using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistance;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace RecruitmentPipeline.Tests.Application
{
    public class AuthServiceTests
    {
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
        private readonly Mock<ITokenHasher> _tokenHasherMock = new();

        private static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // svaki test dobija svoju izolovanu "bazu"
                .Options;

            return new AppDbContext(options);
        }

        // Testiranje registracije
        [Fact]
        public async Task Register_WithNewEmail_CreatesUserAndReturnsTrue()
        {
            await using var context = CreateInMemoryContext();
            var service = new AuthService(
                context, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object, _tokenHasherMock.Object
            );

            _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");

            var request = new RegisterRequestDto { Email = "test@example.com", Password = "Password123!" };

            var result = await service.Register(request);
        
            Assert.True(result);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsFalse()
        {            
            await using var context = CreateInMemoryContext();
            context.Users.Add(new User("test@example.com", "already-hashed"));
            await context.SaveChangesAsync();

            var service = new AuthService(
                context, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object, _tokenHasherMock.Object
            );

            var request = new RegisterRequestDto { Email = "test@example.com", Password = "Password123!" };

            var result = await service.Register(request);

            Assert.False(result);
            _passwordHasherMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);   // hashiranje se ne radi jer je email duplikat
        }

        // Testiranje login-a
        [Fact]
        public async Task Login_WithCorrectData_LoginsUser()
        {
            await using var context = CreateInMemoryContext();
            context.Users.Add(new User("test@test.com", "already-hashed"));
            await context.SaveChangesAsync();

            _passwordHasherMock
                .Setup(h => h.Verify("Password123", "already-hashed")).Returns(true);

            _tokenHasherMock
                .Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-refresh-token");

            _jwtTokenGeneratorMock
                .Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("access-token");

            var service = new AuthService(
                context, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object, _tokenHasherMock.Object
            );        

            var request = new LoginDto { Email = "test@test.com", Password = "Password123" };

            var response = await service.Login(request);

            Assert.NotNull(response.AccessToken);
            Assert.NotNull(response.RefreshToken);
        }

        [Fact]
        public async Task Login_WithInvalidEmail_ThrowsUnauthorizedAccessException()
        {
            await using var context = CreateInMemoryContext();
            context.Users.Add(new User("test@test.com", "already-hashed"));
            await context.SaveChangesAsync();

            var service = new AuthService(
                context, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object, _tokenHasherMock.Object
            );

            var request = new LoginDto { Email = "test@example.com", Password = "Password123" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.Login(request));
            _passwordHasherMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ThrowsUnauthorizedAccessException()
        {
            await using var context = CreateInMemoryContext();
            context.Users.Add(new User("test@test.com", "already-hashed"));
            await context.SaveChangesAsync();

            _passwordHasherMock
                .Setup(h => h.Verify("Password123", "already-hashed")).Returns(true);

            var service = new AuthService(
                context, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object, _tokenHasherMock.Object
            );

            var request = new LoginDto { Email = "test@test.com", Password = "Test1234" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.Login(request));
        }
    }
}