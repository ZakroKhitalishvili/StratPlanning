using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Configurations.Extensions;

namespace Core.Configurations
{
    class IssueOptionAnswerToResourceConfiguration : IEntityTypeConfiguration<IssueOptionAnswerToResource>
    {
        public void Configure(EntityTypeBuilder<IssueOptionAnswerToResource> builder)
        {

            builder.HasOne(x => x.IssueOptionAnswer)
                .WithMany(s => s.IssueOptionAnswersToResources)
                .HasForeignKey(x => x.IssueOptionAnswerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Resource)
                .WithMany(s => s.IssueOptionAnswersToResources)
                .HasForeignKey(x => x.ResourceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
