using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWalker.Domain.Entities;

namespace NetWalker.Infrastructure.Persistence;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Token)
            .IsRequired()
            .HasMaxLength(256); 

        builder.HasIndex(s => s.Token)
            .IsUnique();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.ExpiresAt)
            .IsRequired();

        builder.Property(s => s.IsRevoked)
            .IsRequired();
    }
}