﻿using Core.Constants;
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

            builder.Property(x => x.HasPosition)
                .HasDefaultValue(false);

            builder.Property(x => x.HasValue)
                .HasDefaultValue(false);

            builder.Property(x => x.HasStakeholderCategory)
                .HasDefaultValue(false);

            builder.Property(x => x.HasStakeholderCriteria)
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

            builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        }
    }
}
