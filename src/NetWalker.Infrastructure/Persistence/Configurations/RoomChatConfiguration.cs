using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWalker.Domain.Rooms;

namespace NetWalker.Infrastructure.Persistence;

public class RoomChatConfiguration : IEntityTypeConfiguration<RoomChat>
{
    public void Configure(EntityTypeBuilder<RoomChat> builder)
    {
        builder.ToTable("room_chat");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code)
            .IsRequired();

        builder.Property(c => c.Messages)
            .HasColumnName("chat_msg")
            .HasColumnType("varchar[]")
            .HasConversion(
                v => v.ToArray(),
                v => v.ToList())
            .IsRequired();
    }
}