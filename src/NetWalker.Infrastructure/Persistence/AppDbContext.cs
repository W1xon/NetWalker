using Microsoft.EntityFrameworkCore;
using NetWalker.Domain;
using NetWalker.Domain.Rooms;
using NetWalker.Domain.Users;

namespace NetWalker.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<RoomChat> Chats { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext>options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}