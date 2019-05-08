using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class StakeholderRatingAnswerConfiguration : IEntityTypeConfiguration<StakeholderRatingAnswer>
    {
        public void Configure(EntityTypeBuilder<StakeholderRatingAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.Property(x => x.Grade)
                .IsRequired(false);

            builder.Property(x => x.Priority)
               .IsRequired(true);

            builder.HasOne(x => x.Stakeholder)
                .WithMany(s => s.StakeholderRatingAnswers)
                .HasForeignKey(x => x.StakeholderId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.StakeholderRatingAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.StakeholderRatingAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
