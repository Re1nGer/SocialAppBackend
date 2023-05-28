using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations
{
    public class UserMessageConfiguration : IEntityTypeConfiguration<UserMessage>
    {
        public void Configure(EntityTypeBuilder<UserMessage> builder)
        {
            builder.HasOne(x => x.TargetUser)
                .WithMany(x => x.UserMessageTargets)
                .HasForeignKey(x => x.TargetId);

            builder.HasOne(x => x.SourceUser)
                .WithMany(x => x.UserMessageSources)
                .HasForeignKey(x => x.SourceId);

            builder.HasOne(x => x.UserChat)
                .WithMany(x => x.Messages);
        }
    }
}
