using Domain.Entities;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Candidate> Candidates { get; }
        DbSet<Position> Positions { get; }
        DbSet<Domain.Entities.Application> Applications { get; }
        DbSet<InterviewStage> InterviewStages { get; }
        DbSet<Feedback> Feedbacks { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
