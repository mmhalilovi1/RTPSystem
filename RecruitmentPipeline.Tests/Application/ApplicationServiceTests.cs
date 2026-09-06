using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Moq;
using ApplicationEntity = Domain.Entities.Application;

namespace RecruitmentPipeline.Tests.Application
{
    public class ApplicationServiceTests
    {
        private static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static Position CreateOpenPosition()
        {
            var position = new Position(
                title: "Junior Software Engineer",
                description: "Opis",
                location: "Sarajevo",
                emplType: EmploymentType.Internship,
                skills: new List<Skill>(),
                experience: 0,
                deadline: DateTime.UtcNow.AddDays(30));

            position.Publish();
            return position;
        }

        private static Candidate CreateEligibleCandidate(Guid userId)
        {
            var candidate = new Candidate(userId, "Test Kandidat", "061111222", 1, new List<Skill>());
            candidate.UploadResume("https://example.com/cv.pdf");
            return candidate;
        }

        [Fact]
        public async Task SubmitApplicationAsync_WhenEligible_CreatesApplication()
        {
            await using var context = CreateInMemoryContext();

            var userId = Guid.NewGuid();
            var position = CreateOpenPosition();
            var candidate = CreateEligibleCandidate(userId);

            context.Positions.Add(position);
            context.Candidates.Add(candidate);
            await context.SaveChangesAsync();

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.Setup(c => c.UserId).Returns(userId);

            var service = new ApplicationService(context, currentUserMock.Object);
            var request = new SubmitApplicationRequestDto { PositionId = position.Id };

            var result = await service.SubmitApplicationAsync(request);

            Assert.Equal(candidate.Id, result.CandidateId);
            Assert.Equal(position.Id, result.PositionId);
            Assert.Equal(1, await context.Applications.CountAsync());
        }

        [Fact]
        public async Task SubmitApplicationAsync_WhenAlreadyApplied_ThrowsInvalidOperationException()
        {
            await using var context = CreateInMemoryContext();

            var userId = Guid.NewGuid();
            var position = CreateOpenPosition();
            var candidate = CreateEligibleCandidate(userId);

            context.Positions.Add(position);
            context.Candidates.Add(candidate);
            await context.SaveChangesAsync();

            context.Applications.Add(new ApplicationEntity(candidate.Id, position.Id));
            await context.SaveChangesAsync();

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.Setup(c => c.UserId).Returns(userId);

            var service = new ApplicationService(context, currentUserMock.Object);
            var request = new SubmitApplicationRequestDto { PositionId = position.Id };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.SubmitApplicationAsync(request));
        }
    }
}
