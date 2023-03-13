using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class GroupFollowerConfiguration : IEntityTypeConfiguration<GroupFollower>
    {
        public void Configure(EntityTypeBuilder<GroupFollower> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.GroupFollowers)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Group)
                .WithMany(x => x.GroupFollowers)
                .HasForeignKey(x => x.GroupId);
        }
    }
}
