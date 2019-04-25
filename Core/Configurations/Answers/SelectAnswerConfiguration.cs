using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class SelectAnswerConfiguration : IEntityTypeConfiguration<SelectAnswer>
    {
        public void Configure(EntityTypeBuilder<SelectAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.Property(x => x.AltOption)
                .IsRequired(false)
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.HasOne(x => x.Option)
                .WithMany(s => s.SelectAnswers)
                .HasForeignKey(x => x.OptionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Issue)
                .WithMany(s => s.SelectAnswers)
                .HasForeignKey(x => x.IssueId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.StepAnswer)
                .WithMany(s => s.SelectAnswers)
                .HasForeignKey(x => x.StepAnswerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.SelectAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
