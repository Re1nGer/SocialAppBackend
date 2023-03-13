
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
        }
    }
}
