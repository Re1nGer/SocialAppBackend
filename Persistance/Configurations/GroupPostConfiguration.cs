
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class GroupPostConfiguration : IEntityTypeConfiguration<GroupPost>
    {
        public void Configure(EntityTypeBuilder<GroupPost> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.GroupPosts)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Group)
                .WithMany(x => x.GroupPosts)
                .HasForeignKey(x => x.GroupId);
        }
    }
}
