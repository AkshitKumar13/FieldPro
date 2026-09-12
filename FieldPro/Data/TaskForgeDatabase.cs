using Microsoft.EntityFrameworkCore;
using TaskForge.Models;
using TaskForge.Services;
using TaskItemStatus = TaskForge.Models.TaskStatus;

namespace TaskForge.Data;

public sealed class TaskForgeDatabase : ITaskForgeDataService
{
    private readonly IDbContextFactory<TaskForgeDbContext> _contextFactory;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public TaskForgeDatabase(IDbContextFactory<TaskForgeDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        await _initializationLock.WaitAsync();
        try
        {
            if (_initialized) return;

            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();

            if (!await db.Users.AnyAsync())
            {
                await SeedAsync(db);
            }

            await db.Documents
                .Where(document => document.OriginalFileName == "project-brief.pdf" && document.FilePath == "")
                .ExecuteUpdateAsync(setters => setters.SetProperty(document => document.FilePath, "project-brief.pdf"));

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async Task<List<User>> GetUsersAsync()
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().OrderBy(user => user.DisplayName).ToListAsync();
    }

    public async Task<User?> GetUserAsync(int id)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.UserId == id);
    }

    public async Task<List<TaskItem>> GetTasksAsync(int userId)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Tasks.AsNoTracking().Where(task => task.AssignedUserId == userId).OrderBy(task => task.DueDate).ToListAsync();
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Tasks.AsNoTracking().OrderBy(task => task.DueDate).ToListAsync();
    }

    public async Task<int> AddTaskAsync(TaskItem task, int assignedByUserId)
    {
        await EnsureCanAssignTasksAsync(assignedByUserId);
        await using var db = await _contextFactory.CreateDbContextAsync();
        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        db.Notifications.Add(new Notification
        {
            UserId = task.AssignedUserId,
            Title = "Task assigned",
            Message = $"The task '{task.Title}' has been assigned to you."
        });
        await db.SaveChangesAsync();
        return task.TaskId;
    }

    public async Task AssignTaskAsync(int taskId, int userId, int assignedByUserId)
    {
        await EnsureCanAssignTasksAsync(assignedByUserId);
        await using var db = await _contextFactory.CreateDbContextAsync();
        var task = await db.Tasks.FirstOrDefaultAsync(item => item.TaskId == taskId);
        if (task is null) return;

        task.AssignedUserId = userId;
        db.Notifications.Add(new Notification
        {
            UserId = userId,
            Title = "Task assigned",
            Message = $"The task '{task.Title}' has been assigned to you."
        });
        await db.SaveChangesAsync();
    }

    public async Task<List<Project>> GetProjectsAsync(int userId)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        var memberProjectIds = await db.ProjectMembers
            .Where(member => member.UserId == userId)
            .Select(member => member.ProjectId)
            .ToListAsync();
        return await db.Projects.AsNoTracking()
            .Where(project => project.ProjectManagerId == userId || memberProjectIds.Contains(project.ProjectId))
            .OrderBy(project => project.Name)
            .ToListAsync();
    }

    public async Task<List<ProjectMember>> GetProjectMembersAsync(int projectId)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.ProjectMembers.AsNoTracking().Where(member => member.ProjectId == projectId).ToListAsync();
    }

    public async Task<List<Announcement>> GetAnnouncementsAsync()
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Announcements.AsNoTracking().OrderByDescending(item => item.PublishDate).ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsAsync(int userId)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Notifications.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.NotificationId).ToListAsync();
    }

    public async Task<List<TaskForgeDocument>> GetDocumentsAsync(int userId)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Documents.AsNoTracking().OrderByDescending(document => document.UploadedDate).ToListAsync();
    }

    public async Task UpdateTaskStatusAsync(int taskId, TaskItemStatus status)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        await db.Tasks.Where(task => task.TaskId == taskId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(task => task.Status, status));
    }

    public async Task MarkNotificationReadAsync(int notificationId)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        await db.Notifications.Where(item => item.NotificationId == notificationId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.IsRead, true));
    }

    public async Task UpdateUserAsync(User user)
    {
        await InitializeAsync();
        await using var db = await _contextFactory.CreateDbContextAsync();
        db.Users.Update(user);
        await db.SaveChangesAsync();
    }

    private async Task EnsureCanAssignTasksAsync(int userId)
    {
        var user = await GetUserAsync(userId);
        if (user is not { Role: UserRole.ProjectManager or UserRole.TeamLead or UserRole.Administrator })
        {
            throw new UnauthorizedAccessException("Only managers, team leads, and administrators can assign tasks.");
        }
    }

    private static async Task SeedAsync(TaskForgeDbContext db)
    {
        db.Users.AddRange(
            new User { UserId = 1, Email = "admin@taskforge.app", DisplayName = "System Administrator", Department = "IT", JobTitle = "Administrator", Role = UserRole.Administrator },
            new User { UserId = 2, Email = "mohan@taskforge.app", DisplayName = "Mohan  singh", Department = "Engineering", JobTitle = "Project Manager", Role = UserRole.ProjectManager },
            new User { UserId = 3, Email = "mayank@taskforge.app", DisplayName = "Mayank Sharma", Department = "Engineering", JobTitle = "Team Lead", Role = UserRole.TeamLead },
            new User { UserId = 4, Email = "akshit@taskforge.app", DisplayName = "Akshit Kumar", Department = "Engineering", JobTitle = "Software Engineer", Role = UserRole.Employee });
        db.Projects.Add(new Project { ProjectId = 1, Name = "TaskForge Development", Description = "Internal productivity dashboard", ProjectManagerId = 2, Status = ProjectStatus.Active, TargetCompletionDate = DateTime.UtcNow.AddDays(60) });
        db.Tasks.AddRange(
            new TaskItem { TaskId = 1, Title = "Design database schema", Description = "Create entity relationship diagram", Priority = TaskPriority.High, Status = TaskItemStatus.Completed, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(-5) },
            new TaskItem { TaskId = 2, Title = "Implement authentication", Description = "Set up role-based demo access", Priority = TaskPriority.Critical, Status = TaskItemStatus.InProgress, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(5) },
            new TaskItem { TaskId = 3, Title = "Create UI mockups", Description = "Design the main dashboard pages", Priority = TaskPriority.Medium, Status = TaskItemStatus.NotStarted, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(10) });
        db.ProjectMembers.AddRange(
            new ProjectMember { ProjectId = 1, UserId = 3, Role = "Team Lead" },
            new ProjectMember { ProjectId = 1, UserId = 4, Role = "Developer" });
        db.Announcements.Add(new Announcement { Title = "Welcome to TaskForge", Content = "Manage tasks, projects, documents, and team coordination in one place." });
        db.Notifications.Add(new Notification { UserId = 4, Title = "New task assigned", Message = "Implement authentication was assigned to you." });
        db.Documents.Add(new TaskForgeDocument { Title = "Project brief", Category = "Planning", OriginalFileName = "project-brief.pdf", ContentType = "application/pdf", FilePath = "project-brief.pdf", FileSize = 128000, UploadedByUserId = 4, ProjectId = 1 });
        await db.SaveChangesAsync();
    }
}
