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
                .IsRequired(false);

            builder.Property(x => x.HowItWillBeDone)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(x => x.IssueOptionAnswer)
                .WithMany(s => s.PreparingAnswers)
                .HasForeignKey(x => x.IssueOptionAnswerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.PreparingAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.PreparingAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
