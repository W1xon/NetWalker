using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWalker.Domain;
using NetWalker.Domain.Users;

namespace NetWalker.Infrastructure.Persistence;

public class PlayerStatsConfiguration : IEntityTypeConfiguration<PlayerStats>
{
    public void Configure(EntityTypeBuilder<PlayerStats> builder)
    {
        builder.ToTable("player_stats");
        
        builder.HasKey(s => s.UserId); 
    }
}
