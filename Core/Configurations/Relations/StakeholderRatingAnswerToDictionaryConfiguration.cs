using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    class StakeholderRatingAnswerToDictionaryConfiguration : IEntityTypeConfiguration<StakeholderRatingAnswerToDictionary>
    {
        public void Configure(EntityTypeBuilder<StakeholderRatingAnswerToDictionary> builder)
        {
            builder.Property(x => x.Rate)
                .IsRequired(true);

            builder.HasOne(x => x.StakeholderRatingAnswer)
                .WithMany(s => s.Criteria)
                .HasForeignKey(x => x.StakeholderRatingAnswerId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Criterion)
                .WithMany(s => s.StakeholderRatingAnswersToDictionaries)
                .HasForeignKey(x => x.CriterionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
