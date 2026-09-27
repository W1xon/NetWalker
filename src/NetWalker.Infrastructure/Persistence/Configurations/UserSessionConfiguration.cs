using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWalker.Domain.Users;

namespace NetWalker.Infrastructure.Persistence;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.IpAddress)
            .IsRequired();
        
        builder.Property(s => s.DeviceType)
            .IsRequired();
        
        builder.Property(s => s.Os)
            .IsRequired();
        
        builder.Property(s => s.TokenHash)
            .IsRequired()
            .HasMaxLength(256); 

        builder.HasIndex(s => s.TokenHash)
            .IsUnique();

        builder.Property(s => s.PreviousTokenHash)
            .HasMaxLength(256);

        builder.Property(s => s.PreviousTokenExpiresAt);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.ExpiresAt)
            .IsRequired();

        builder.Property(s => s.IsRevoked)
            .IsRequired();
    }
}