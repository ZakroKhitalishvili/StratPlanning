using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class StepTaskAnswerConfiguration : IEntityTypeConfiguration<StepTaskAnswer>
    {
        public void Configure(EntityTypeBuilder<StepTaskAnswer> builder)
        {

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.FirstName)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.IsDefinitive)
               .IsRequired()
               .HasDefaultValue(false);

            builder.HasOne(x => x.UserToPlan)
               .WithMany(s => s.StepTaskAnswers)
               .HasForeignKey(x => x.UserToPlanId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.StepTask)
               .WithMany(s => s.StepTaskAnswers)
               .HasForeignKey(x => x.StepTaskId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

            builder.HasOne(x => x.Plan)
                .WithMany(x => x.AdminStepTaskAnswers)
                .HasForeignKey(x => x.PlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
