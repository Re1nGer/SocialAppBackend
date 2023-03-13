
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class GroupMetaConfiguration : IEntityTypeConfiguration<GroupMeta>
    {
        public void Configure(EntityTypeBuilder<GroupMeta> builder)
        {
            builder.HasOne(x => x.Group)
                .WithMany(x => x.GroupMetas)
                .HasForeignKey(x => x.GroupId);
        }
    }
}
