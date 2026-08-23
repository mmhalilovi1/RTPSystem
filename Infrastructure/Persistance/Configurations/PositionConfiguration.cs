using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder) 
        {
            builder.ToTable("Positions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(4000);

            builder.Property(p => p.Location)
                .HasMaxLength(200);

            // enum eksplicitno čuvao kao string radi čitljivosti u bazi
            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.EmploymentType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.RequiredExperience)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.ClosedAt)
                .IsRequired(false);

            builder.Property(p => p.Deadline)
                .IsRequired();

            builder.HasMany(p => p.RequiredSkills)
                .WithMany()
                .UsingEntity(j => j.ToTable("PositionSkills"));

            //  Treba dodati relationship sa Application entitetom.
        }
    }
}
