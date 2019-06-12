using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Configurations
{
    class IntroductionConfiguration : IEntityTypeConfiguration<Introduction>
    {
        public void Configure(EntityTypeBuilder<Introduction> builder)
        {
            builder.Property(x => x.Step)
              .IsRequired()
              .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

            builder.HasOne(x => x.Video)
                .WithOne(s => s.Introduction)
                .HasForeignKey<Introduction>(x => x.VideoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
