﻿using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class StepTaskAnswerConfiguration : IEntityTypeConfiguration<StepTaskAnswer>
    {
        public void Configure(EntityTypeBuilder<StepTaskAnswer> builder)
        {

            builder.Property(x => x.Email)
                .IsRequired(false)
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.FirstName)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.LastName)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.IsDefinitive)
               .IsRequired()
               .HasDefaultValue(false);

            builder.HasOne(x => x.UserStepResult)
               .WithMany(s => s.StepTaskAnswers)
               .HasForeignKey(x => x.UserStepResultId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserToPlan)
               .WithMany(s => s.StepTaskAnswers)
               .HasForeignKey(x => x.UserToPlanId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.StepTask)
               .WithMany(s => s.StepTaskAnswers)
               .HasForeignKey(x => x.StepTaskId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

        }
    }
}
