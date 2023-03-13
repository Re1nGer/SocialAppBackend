
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasOne(x => x.UserCreatedBy)
                .WithMany(x => x.GroupsCreatedBy)
                .HasForeignKey(x => x.CreatedBy);

            builder.HasOne(x => x.UserUpdatedBy)
                .WithMany(x => x.GroupsUpdatedBy)
                .HasForeignKey(x => x.UpdatedBy);

        }
    }
}
