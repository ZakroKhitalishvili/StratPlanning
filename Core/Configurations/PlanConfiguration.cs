using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;
using Core.Constants;

namespace Core.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired(false);

            builder.Property(x => x.IsWithActionPlan)
                .IsRequired(false);

            builder.Property(x => x.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property(x => x.CreatedAt)
              .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.UpdatedBy)
                .IsRequired();

            builder.HasMany(x => x.UsersToPlans)
                .WithOne(s => s.Plan)
                .HasForeignKey(s => s.PlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
