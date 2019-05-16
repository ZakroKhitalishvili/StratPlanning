using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Core.Configurations
{
    class StepTaskConfiguration : IEntityTypeConfiguration<StepTask>
    {
        public void Configure(EntityTypeBuilder<StepTask> builder)
        {
            builder.Property(x => x.Step)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.HasOne(x => x.Plan)
                .WithMany(s => s.StepTasks)
                .HasForeignKey(x => x.PlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Schedule)
                .IsRequired(false);

            builder.Property(x => x.Remind)
               .IsRequired(false);

            builder.Property(x => x.IsCompleted)
              .IsRequired()
              .HasDefaultValue(false);

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
