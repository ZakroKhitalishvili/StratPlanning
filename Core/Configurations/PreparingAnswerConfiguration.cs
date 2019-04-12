﻿using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class PreparingAnswerConfiguration : IEntityTypeConfiguration<PreparingAnswer>
    {
        public void Configure(EntityTypeBuilder<PreparingAnswer> builder)
        {

            builder.AnswersBaseConfigure();

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.HowItWillBeDone)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(x => x.IssueOptionAnswer)
                .WithOne(s => s.PreparingAnswer)
                .HasForeignKey<PreparingAnswer>(x => x.IssueOptionAnswerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserToPlan)
                .WithMany(s => s.PreparingAnswers)
                .HasForeignKey(x => x.UserToPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.PreparingAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
