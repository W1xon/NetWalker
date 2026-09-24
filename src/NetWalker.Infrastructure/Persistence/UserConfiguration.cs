using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWalker.Domain;
using NetWalker.Domain.Users;

namespace NetWalker.Infrastructure.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(u => u.Id);
        builder.HasOne(u => u.Stats)
            .WithOne()
            .HasForeignKey<PlayerStats>(s => s.UserId);

        builder.Property(u => u.Nick)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(u => u.Nick)
            .IsUnique();
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        builder.Property(u => u.CreatedTime)
            .IsRequired();
    }
}