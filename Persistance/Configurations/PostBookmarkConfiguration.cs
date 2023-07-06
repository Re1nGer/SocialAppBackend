using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class PostBookmarkConfiguration : IEntityTypeConfiguration<PostBookmark>
{
    public void Configure(EntityTypeBuilder<PostBookmark> builder)
    {
        builder.HasOne(x => x.UserPost)
            .WithMany(x => x.PostBookmarks);

        builder.HasOne(item => item.User)
            .WithMany(item => item.PostBookmarks);
    }
}