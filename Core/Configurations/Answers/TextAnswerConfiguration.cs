using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class TextAnswerConfiguration : IEntityTypeConfiguration<TextAnswer>
    {
        public void Configure(EntityTypeBuilder<TextAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.IsIssue)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(x => x.IsStakeholder)
               .IsRequired()
               .HasDefaultValue(false);


            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.TextAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.TextAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
