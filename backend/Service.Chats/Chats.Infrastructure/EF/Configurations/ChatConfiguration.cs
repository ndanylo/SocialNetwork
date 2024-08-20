using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Chats.Domain.Entities;
using Chats.Domain.ValueObjects;

namespace Chats.EF.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(new ChatIdValueConverter())
                .IsRequired();

            builder.Property(e => e.Name)
                .HasConversion(
                    name => name.Name,
                    nameString => ChatRoomName.Create(nameString).Value)
                .IsRequired();

            builder.Property(e => e.IsGroupChat).IsRequired();

            builder.HasMany(e => e.Messages)
                .WithOne(m => m.ChatRoom)
                .HasForeignKey(m => m.ChatId);
        }
    }
}
