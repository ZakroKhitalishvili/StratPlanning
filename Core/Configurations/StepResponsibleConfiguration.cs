using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    class StepResponsibleConfiguration : IEntityTypeConfiguration<StepResponsible>
    {
        public void Configure(EntityTypeBuilder<StepResponsible> builder)
        {
            builder.Property(x => x.Step)
               .IsRequired()
               .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.HasOne(x => x.UserToPlan)
                .WithMany(s => s.StepResponsibles)
                .HasForeignKey(x => x.UserToPlanId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
