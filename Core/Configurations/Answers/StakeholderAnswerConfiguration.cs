using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class StakeholderAnswerConfiguration : IEntityTypeConfiguration<StakeholderAnswer>
    {
        public void Configure(EntityTypeBuilder<StakeholderAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.Property(x => x.IsInternal)
                .IsRequired(true);

            builder.Property(x => x.LastName)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.FirstName)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Email)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.HasOne(x => x.User)
                .WithMany(s => s.StakeholderAnswers)
                .HasForeignKey(x => x.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.StakeholderAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.StakeholderAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
