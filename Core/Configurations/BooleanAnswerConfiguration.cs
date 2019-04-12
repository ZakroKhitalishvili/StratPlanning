using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class BooleanAnswerConfiguration : IEntityTypeConfiguration<BooleanAnswer>
    {
        public void Configure(EntityTypeBuilder<BooleanAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.HasOne(x => x.Resource)
                .WithOne(s => s.BooleanAnswer)
                .HasForeignKey<BooleanAnswer>(x => x.ResourceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Answer)
                .IsRequired();

            builder.HasOne(x => x.UserToPlan)
                .WithMany(s => s.BooleanAnswers)
                .HasForeignKey(x => x.UserToPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Question)
                .WithMany(s => s.BooleanAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
