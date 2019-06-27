using Core.Constants;
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

            builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Role)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

            builder.Property(x => x.IsDeleted)
              .HasDefaultValue(false);

            builder.Property(x => x.EmailBackUp)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Token)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.HashMaxLength);

            builder.Property(x => x.TokenExpiration)
                .IsRequired(false);

            builder.Property(x => x.Password)
               .IsRequired()
               .HasMaxLength(EntityConfigs.HashMaxLength);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

            builder.HasOne(x => x.Position)
                .WithMany(s => s.Users)
                .HasForeignKey(x => x.PositionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
