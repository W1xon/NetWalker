using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWalker.Domain.Rooms;

namespace NetWalker.Infrastructure.Persistence;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.SessionCode)
            .IsUnique();

        builder.Property(r => r.TicketCode);
        builder.Property(r => r.IsTicketClaimed);
        builder.Property(r => r.HostId);
        
        builder.Property(r => r.MaxPlayers)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.CreatedTime)
            .IsRequired();

        builder.Property(r => r.LastHeartbeat)
            .IsRequired();

        builder.Property(r => r.PlayerIds)
            .HasColumnName("player_ids")
            .HasColumnType("uuid[]")
            .HasConversion(
                v => v.ToArray(),
                v => v.ToList())
            .IsRequired();
    }
}