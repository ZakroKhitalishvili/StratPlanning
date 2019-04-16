using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    class UserToPlanConfiguration : IEntityTypeConfiguration<UserToPlan>
    {
        public void Configure(EntityTypeBuilder<UserToPlan> builder)
        {

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.HasOne(x => x.User)
                .WithMany(s => s.UsersToPlans)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Plan)
               .WithMany(s => s.UsersToPlans)
               .HasForeignKey(x => x.PlanId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
