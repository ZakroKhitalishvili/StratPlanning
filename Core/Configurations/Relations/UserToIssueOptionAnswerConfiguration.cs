using Core.Constants;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configurations
{
    class UserToIssueOptionAnswerConfiguration : IEntityTypeConfiguration<UserToIssueOptionAnswer>
    {
        public void Configure(EntityTypeBuilder<UserToIssueOptionAnswer> builder)
        {

            builder.HasOne(x => x.User)
                .WithMany(s => s.UsersToIssueOptionAnswers)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.IssueOptionAnswer)
               .WithMany(s => s.UsersToIssueOptionAnswers)
               .HasForeignKey(x => x.IssueOptionAnswerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
