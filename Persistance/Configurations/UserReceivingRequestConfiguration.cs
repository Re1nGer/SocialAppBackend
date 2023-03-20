using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserReceivingRequestConfiguration : IEntityTypeConfiguration<UserReceivingRequest>
    {
        public void Configure(EntityTypeBuilder<UserReceivingRequest> builder)
        {
            builder.HasOne(item => item.TargetUser)
                .WithMany(item => item.UserReceivingRequests);

            builder.HasOne(item => item.UserRequest)
                .WithOne(item => item.UserReceivingRequest)
                .HasForeignKey<UserReceivingRequest>(item => item.UserRequestId);
        }
    }
}
