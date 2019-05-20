using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class FileAnswerConfiguration : IEntityTypeConfiguration<FileAnswer>
    {
        public void Configure(EntityTypeBuilder<FileAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.HasOne(x => x.File)
                .WithMany(s => s.FileAnswers)
                .HasForeignKey(x => x.FileId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserStepResult)
                .WithMany(s => s.FileAnswers)
                .HasForeignKey(x => x.UserStepResultId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.FileAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
