using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.ToTable("Candidates");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.PhoneNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.ResumeUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.YearsOfExperience)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Candidate>(c => c.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Skills)
                .WithMany()
                .UsingEntity(j => j.ToTable("CandidateSkills"));
        }
    }
}
