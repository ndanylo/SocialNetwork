using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Chats.Domain.Entities;
namespace Chats.EF.Configurations
{
    public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
    {
        public void Configure(EntityTypeBuilder<ChatUser> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(new ChatUserIdValueConverter())
                .IsRequired();

            builder.Property(cu => cu.UserId)
                .HasConversion(new UserIdValueConverter())
                .IsRequired();

            builder.Property(cu => cu.ChatId)
                .HasConversion(new ChatIdValueConverter())
                .IsRequired();

            builder
                .HasMany(cu => cu.ReadMessages)
                .WithMany(m => m.ReadBy)
                .UsingEntity<Dictionary<string, object>>(
                    "MessageReadByChatUser",
                    j => j.HasOne<Message>()
                          .WithMany()
                          .HasForeignKey("MessageId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<ChatUser>()
                          .WithMany()
                          .HasForeignKey("ChatUserId")
                          .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
