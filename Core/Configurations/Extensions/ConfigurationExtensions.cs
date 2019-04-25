using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;



namespace Core.Configurations.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void  AnswersBaseConfigure<T> (this EntityTypeBuilder<T> builder) where T:AbstractAnswer
        {

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
