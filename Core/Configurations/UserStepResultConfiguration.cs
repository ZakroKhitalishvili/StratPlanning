using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class UserStepResultConfiguration : IEntityTypeConfiguration<UserStepResult>
    {
        public void Configure(EntityTypeBuilder<UserStepResult> builder)
        {

            builder.Property(x => x.Step)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.IsSubmitted)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(x => x.IsDefinitive)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(x => x.IsFinal)
              .IsRequired(false);

            builder.HasOne(x => x.UserToPlan)
               .WithMany(s => s.UserStepResults)
               .HasForeignKey(x => x.UserToPlanId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Plan)
               .WithMany(s => s.AdminStepResults)
               .HasForeignKey(x => x.PlanId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

        }
    }
}
