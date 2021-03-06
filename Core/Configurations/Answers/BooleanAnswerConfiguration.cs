﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class BooleanAnswerConfiguration : IEntityTypeConfiguration<BooleanAnswer>
    {
        public void Configure(EntityTypeBuilder<BooleanAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.HasOne(x => x.Resource)
                .WithMany(s => s.BooleanAnswers)
                .HasForeignKey(x => x.ResourceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Answer)
                .IsRequired();

            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.BooleanAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.BooleanAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
