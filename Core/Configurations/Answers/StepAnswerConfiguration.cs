﻿using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class StepAnswerConfiguration : IEntityTypeConfiguration<StepAnswer>
    {
        public void Configure(EntityTypeBuilder<StepAnswer> builder)
        {

            builder.Property(x => x.Step)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.IsSubmitted)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(x => x.IsFinal)
               .IsRequired()
               .HasDefaultValue(false);

            builder.HasOne(x => x.UserToPlan)
               .WithMany(s => s.StepAnswers)
               .HasForeignKey(x => x.UserToPlanId)
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
