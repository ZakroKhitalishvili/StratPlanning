using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Configurations
{
    class DictionaryConfiguration : IEntityTypeConfiguration<Dictionary>
    {
        public void Configure(EntityTypeBuilder<Dictionary> builder)
        {

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.UpdatedBy)
                .IsRequired();

            builder.HasMany(x => x.Users)
                .WithOne(s => s.Position)
                .HasForeignKey(s => s.PositionId)
                .IsRequired(false);

            builder.HasMany(x => x.StakeholderRatingAnswersToDictionaries)
                .WithOne(s => s.Criterion)
                .HasForeignKey(s => s.CriterionId)
                .IsRequired();
        }
    }
}
