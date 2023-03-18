using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserPostConfiguration : IEntityTypeConfiguration<UserPost>
    {
        public void Configure(EntityTypeBuilder<UserPost> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserPosts)
                .HasForeignKey(x => x.UserId);

            builder.HasMany(item => item.Comments)
                   .WithOne(item => item.UserPost)
                   .HasForeignKey(x => x.PostId);

            builder.HasMany(item => item.Likes)
                   .WithOne(item => item.UserPost)
                   .HasForeignKey(x => x.PostId);
        }
    }
}
