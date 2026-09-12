using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;
using TaskForge.Models;

namespace TaskForge.Data;

public sealed class TaskForgeDbContext : DbContext
{
    public TaskForgeDbContext(DbContextOptions<TaskForgeDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<TaskForgeDocument> Documents => Set<TaskForgeDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable(nameof(User));
            entity.HasKey(user => user.UserId);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(255);
            entity.Ignore(user => user.DisplayLabel);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable(nameof(TaskItem));
            entity.HasKey(task => task.TaskId);
            entity.Ignore(task => task.AssignedUserName);
        });

        modelBuilder.Entity<Project>().ToTable(nameof(Project)).HasKey(project => project.ProjectId);
        modelBuilder.Entity<ProjectMember>().ToTable(nameof(ProjectMember)).HasKey(member => member.ProjectMemberId);
        modelBuilder.Entity<Announcement>().ToTable(nameof(Announcement)).HasKey(item => item.AnnouncementId);
        modelBuilder.Entity<Notification>().ToTable(nameof(Notification)).HasKey(item => item.NotificationId);
        modelBuilder.Entity<TaskForgeDocument>().ToTable(nameof(TaskForgeDocument)).HasKey(document => document.DocumentId);
    }

    public static string GetDatabasePath() => Path.Combine(
        FileSystem.AppDataDirectory,
        "taskforge-ef.db3");
}
