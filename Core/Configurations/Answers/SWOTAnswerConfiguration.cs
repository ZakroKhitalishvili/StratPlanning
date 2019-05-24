using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class SWOTAnswerConfiguration : IEntityTypeConfiguration<SWOTAnswer>
    {
        public void Configure(EntityTypeBuilder<SWOTAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.SWOTAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.SWOTAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
