using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistance
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Position> Positions => Set<Position>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Candidate> Candidates => Set<Candidate>();
        public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();
        public DbSet<InterviewStage> InterviewStages => Set<InterviewStage>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Skill> Skills => Set<Skill>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<UtcDateTimeConverter>();
        }
    }
}
