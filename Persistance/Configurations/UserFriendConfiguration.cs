using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserFriendConfiguration : IEntityTypeConfiguration<UserFriend>
    {
        public void Configure(EntityTypeBuilder<UserFriend> builder)
        {
            builder.HasOne(x => x.TargetUser)
                .WithMany(x => x.UserFriendTargets)
                .HasForeignKey(x => x.TargetId);

            builder.HasOne(x => x.SourceUser)
                .WithMany(x => x.UserFriendSources)
                .HasForeignKey(x => x.SourceId);
        }
    }
}
