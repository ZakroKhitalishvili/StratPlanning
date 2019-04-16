using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(EntityConfigs.TextMaxLength);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

        }
    }
}
