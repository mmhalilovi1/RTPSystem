using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace RecruitmentPipeline.Tests.Application
{
    public class PositionServiceTests
    {      
        private static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Create_WithNoExistingSkills_CreatesPosition()
        {
            await using var context = CreateInMemoryContext();
            var service = new PositionService(context);

            var request = new PositionRequestDto
            {
                Title = "Junior Backend Dev",
                Description = "Neki opis",
                Location = "Sarajevo",
                EmploymentType = EmploymentType.FullTime,
                RequiredExperience = 2,
                Deadline = DateTime.UtcNow.AddDays(30),
                RequiredSkills = new List<string> { "C++" } 
            };

            var response = await service.CreateAsync(request);

            Assert.Equal(1, await context.Positions.CountAsync());
            Assert.Equal(1, await context.Skills.CountAsync());
        }

        [Fact]
        public async Task Create_WithExistingSkills_CreatesPositionWithoutDuplicateSkills()
        {
            await using var context = CreateInMemoryContext();          
            var service = new PositionService(context);

            var request = new PositionRequestDto
            {
                Title = "Junior Backend Dev",
                Description = "Neki opis",
                Location = "Sarajevo",
                EmploymentType = EmploymentType.FullTime,
                RequiredExperience = 2,
                Deadline = DateTime.UtcNow.AddDays(30),
                RequiredSkills = new List<string> { "C++" }
            };

            var response = await service.CreateAsync(request);

            var request_v2 = new PositionRequestDto
            {
                Title = "Nova pozicija",
                Description = "Neki novi opis",
                Location = "Zagreb",
                EmploymentType = EmploymentType.Internship,
                RequiredExperience = 0,
                Deadline = DateTime.UtcNow.AddDays(30),
                RequiredSkills = new List<string> { "C++" }
            };

            var response_v2 = await service.CreateAsync(request_v2);

            Assert.Equal(1, await context.Skills.CountAsync());
        }
    }
}
