using Core.Constants;
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
            builder.AnswersBaseConfigure();

            builder.Property(x => x.Step)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Date)
               .IsRequired();

            builder.Property(x => x.Remind)
               .IsRequired();

            builder.Property(x => x.IsCompleted)
               .IsRequired()
               .HasDefaultValue(false);

            builder.HasOne(x => x.UserToPlan)
                .WithMany(s => s.StepAnswers)
                .HasForeignKey(x => x.UserToPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.StepAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
