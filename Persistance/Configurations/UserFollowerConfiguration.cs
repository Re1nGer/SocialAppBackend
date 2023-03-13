using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserFollowerConfiguration : IEntityTypeConfiguration<UserFollower>
    {
        public void Configure(EntityTypeBuilder<UserFollower> builder)
        {
            builder.HasOne(x => x.TargetUser)
                .WithMany(x => x.UserFollowerTargets)
                .HasForeignKey(x => x.TargetId);

            builder.HasOne(x => x.SourceUser)
                .WithMany(x => x.UserFollowerSources)
                .HasForeignKey(x => x.SourceId);
        }
    }
}
