using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Chats.Domain.Entities;

namespace Chats.EF.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(new MessageIdValueConverter())
                .IsRequired();

            builder.Property(e => e.ChatUserId)
                .HasConversion(new ChatUserIdValueConverter())
                .IsRequired();

            builder.Property(e => e.ChatId)
                .HasConversion(new ChatIdValueConverter())
                .IsRequired();

            builder.Property(e => e.Timestamp)
                .IsRequired();

            builder.HasOne(m => m.ChatRoom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);

            builder.OwnsOne(e => e.Content, b =>
            {
                b.Property(v => v.Content)
                    .HasColumnName("Content")
                    .IsRequired();
            });

            builder
                .HasMany(m => m.ReadBy)
                .WithMany(cu => cu.ReadMessages)
                .UsingEntity<Dictionary<string, object>>(
                    "MessageReadByChatUser",
                    j => j.HasOne<ChatUser>()
                          .WithMany()
                          .HasForeignKey("ChatUserId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Message>()
                          .WithMany()
                          .HasForeignKey("MessageId")
                          .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
