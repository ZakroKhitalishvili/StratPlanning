using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class StrategicIssueAnswerConfiguration : IEntityTypeConfiguration<StrategicIssueAnswer>
    {
        public void Configure(EntityTypeBuilder<StrategicIssueAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.Property(x => x.Why)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Result)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Goal)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Solution)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Ranking)
               .IsRequired();

            builder.HasOne(x => x.Issue)
                .WithMany(s => s.StrategicIssueAnswers)
                .HasForeignKey(x => x.IssueId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.StepAnswer)
                .WithMany(s => s.StrategicIssueAnswers)
                .HasForeignKey(x => x.StepAnswerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.StrategicIssueAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
