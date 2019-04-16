using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Configurations
{
    class IssueOptionAnswerConfiguration : IEntityTypeConfiguration<IssueOptionAnswer>
    {
        public void Configure(EntityTypeBuilder<IssueOptionAnswer> builder)
        {

            builder.Property(x => x.Option)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.IsBestOption)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.Actors)
                .IsRequired(false)
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.HasOne(x => x.Issue)
                .WithMany(x => x.IssueOptionAnswers)
                .HasForeignKey(x => x.IssueId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserToPlan)
                .WithMany(s => s.IssueOptionAnswers)
                .HasForeignKey(x => x.UserToPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.IssueOptionAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
