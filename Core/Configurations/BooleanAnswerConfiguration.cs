using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class BooleanAnswerConfiguration : IEntityTypeConfiguration<BooleanAnswer>
    {
        public void Configure(EntityTypeBuilder<BooleanAnswer> builder)
        {
            builder.AnswersBaseConfigure();

            builder.HasOne(x => x.Resource)
                .WithOne(s => s.BooleanAnswer)
                .HasForeignKey<BooleanAnswer>(x => x.ResourceId)
                .IsRequired(false);

            builder.Property(x => x.Answer)
                .IsRequired();
        }
    }
}
