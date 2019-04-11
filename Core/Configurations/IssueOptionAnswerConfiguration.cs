using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class IssueOptionAnswerConfiguration : IEntityTypeConfiguration<IssueOptionAnswer>
    {
        public void Configure(EntityTypeBuilder<IssueOptionAnswer> builder)
        {

            builder.AnswersBaseConfigure();

            builder.Property(x => x.Option)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.IsBestOption)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.Actors)
                .IsRequired(false)
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.HasOne(x => x.Issue)
                .WithMany(s => s.IssueOptionAnswers)
                .HasForeignKey(s => s.IssueId)
                .IsRequired();




        }
    }
}
