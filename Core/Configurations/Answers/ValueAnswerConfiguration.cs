using Core.Configurations.Extensions;
using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Configurations.Answers
{
    public class ValueAnswerConfiguration : IEntityTypeConfiguration<ValueAnswer>
    {
        public void Configure(EntityTypeBuilder<ValueAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Definition)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.ValueAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.ValueAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
