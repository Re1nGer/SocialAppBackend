using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
    {
        public void Configure(EntityTypeBuilder<UserChat> builder)
        {
            builder.HasOne(item => item.User)
                .WithMany(item => item.UserChats);
        }
    }
}
