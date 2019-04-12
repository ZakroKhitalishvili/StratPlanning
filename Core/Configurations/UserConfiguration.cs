﻿using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.UserName)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Role)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Password)
               .IsRequired()
               .HasMaxLength(EntityConfigs.HashMaxLength);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.UpdatedBy)
                .IsRequired();

            builder.HasOne(x => x.Position)
                .WithMany(s => s.Users)
                .HasForeignKey(x => x.PositionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
