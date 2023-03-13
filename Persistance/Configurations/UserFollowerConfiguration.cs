using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserFollowerConfiguration : IEntityTypeConfiguration<UserFollower>
    {
        public void Configure(EntityTypeBuilder<UserFollower> builder)
        {
            builder.HasOne(x => x.FollowingUser)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowingId);

/*            builder.HasOne(x => x.FollowerUser)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowerId);
*/        }
    }
}
