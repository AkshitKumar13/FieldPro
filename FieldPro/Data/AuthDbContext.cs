using FieldPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;

namespace FieldPro.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();
    }

    public static string GetDatabasePath() => Path.Combine(
        FileSystem.AppDataDirectory,
        "fieldpro-auth.db3");
}
