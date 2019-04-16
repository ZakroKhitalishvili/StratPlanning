using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Core.Configurations
{
    class StepBlockConfiguration : IEntityTypeConfiguration<StepBlock>
    {
        public void Configure(EntityTypeBuilder<StepBlock> builder)
        {
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Step)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.Instruction)
               .IsRequired(false)
               .HasMaxLength(EntityConfigs.TextAreaMaxLength);

            builder.Property(x => x.Order)
              .IsRequired();

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
