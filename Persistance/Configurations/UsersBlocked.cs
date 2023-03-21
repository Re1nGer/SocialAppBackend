using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UsersBlocked : IEntityTypeConfiguration<UserBlocked>
    {
        public void Configure(EntityTypeBuilder<UserBlocked> builder)
        {
            builder.HasOne(item => item.User)
                .WithMany(item => item.UsersBlocked);
        }
    }
}
