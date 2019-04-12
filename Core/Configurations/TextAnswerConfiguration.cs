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


            builder.HasOne(x => x.UserToPlan)
                .WithMany(s => s.TextAnswers)
                .HasForeignKey(x => x.UserToPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.TextAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
