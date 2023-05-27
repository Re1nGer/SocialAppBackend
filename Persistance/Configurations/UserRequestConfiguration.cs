using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserRequestConfiguration : IEntityTypeConfiguration<UserRequest>
    {
        public void Configure(EntityTypeBuilder<UserRequest> builder)
        {
            builder.HasOne(item => item.SendUser)
                .WithMany(item => item.UserRequests)
                .HasForeignKey(item => item.SenderUserId);
        }
    }
}
