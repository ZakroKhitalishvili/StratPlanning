using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Configurations
{
    class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.HasOptions)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(x => x.HasOptions)
              .IsRequired()
              .HasDefaultValue(false);

            builder.Property(x => x.Order)
                .IsRequired();

            builder.Property(x => x.CanSpecifyOther)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

            builder.HasOne(x => x.StepBlock)
                .WithMany(s => s.Questions)
                .HasForeignKey(x => x.StepBlockId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
