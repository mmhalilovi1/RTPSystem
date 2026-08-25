using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance.Configurations
{
    public class InterviewStageConfiguration : IEntityTypeConfiguration<InterviewStage>
    {
        public void Configure(EntityTypeBuilder<InterviewStage> builder)
        {
            builder.ToTable("InterviewStages");

            builder.HasKey(i =>  i.Id);

            builder.Property(i => i.StageType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

            builder.Property(i => i.Outcome)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(i => i.ScheduledAt)
                .IsRequired(false);

            builder.Property(i => i.CompletedAt)
                .IsRequired(false);

            builder.Property(i => i.Notes)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.HasOne<Domain.Entities.Application>()
                .WithMany(a => a.InterviewStages)
                .HasForeignKey(i => i.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
