using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Configurations
{
    class IntroductionConfiguration : IEntityTypeConfiguration<Introduction>
    {
        public void Configure(EntityTypeBuilder<Introduction> builder)
        {

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Step)
              .IsRequired()
              .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Description)
              .IsRequired()
              .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.UpdatedBy)
                .IsRequired();

            builder.HasOne(x => x.Plan)
                .WithMany(s => s.Introductions)
                .HasForeignKey(s => s.PlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Video)
                .WithOne(s => s.Introduction)
                .HasForeignKey<Introduction>(x => x.VideoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
