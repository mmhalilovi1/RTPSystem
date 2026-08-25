using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedback");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Rating)
                .IsRequired();

            builder.Property(f => f.Comments)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.HasOne<InterviewStage>()
                .WithMany()
                .HasForeignKey(f => f.InterviewStageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.AuthorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
