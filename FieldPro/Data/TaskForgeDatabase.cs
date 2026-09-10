using TaskForge.Models;
using Microsoft.Maui.Storage;
using SQLite;
using TaskItemStatus = TaskForge.Models.TaskStatus;

namespace TaskForge.Data;

public sealed class TaskForgeDatabase : TaskForge.Services.ITaskForgeDataService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _initialized;

    public TaskForgeDatabase()
    {
        _database = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "taskforge.db3"));
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        await _database.CreateTableAsync<User>();
        await _database.CreateTableAsync<TaskItem>();
        await _database.CreateTableAsync<Project>();
        await _database.CreateTableAsync<ProjectMember>();
        await _database.CreateTableAsync<Announcement>();
        await _database.CreateTableAsync<Notification>();
        await _database.CreateTableAsync<TaskForgeDocument>();
        try
        {
            await _database.ExecuteAsync("ALTER TABLE TaskForgeDocument ADD COLUMN FilePath TEXT");
        }
        catch (SQLiteException)
        {
            // The column already exists on new databases or after the first migration.
        }
        await _database.ExecuteAsync("UPDATE TaskForgeDocument SET FilePath = ? WHERE OriginalFileName = ? AND (FilePath IS NULL OR FilePath = '')", "project-brief.pdf", "project-brief.pdf");
        if (await _database.Table<User>().CountAsync() == 0) await SeedAsync();
        await _database.ExecuteAsync("UPDATE User SET DisplayName = ? WHERE UserId = ?", "Ali Patel", 2);
        _initialized = true;
    }

    public Task<List<User>> GetUsersAsync() => _database.Table<User>().OrderBy(x => x.DisplayName).ToListAsync();
    public Task<User?> GetUserAsync(int id) => _database.Table<User>().Where(x => x.UserId == id).FirstOrDefaultAsync();
    public Task<List<TaskItem>> GetTasksAsync(int userId) => _database.Table<TaskItem>().Where(x => x.AssignedUserId == userId).OrderBy(x => x.DueDate).ToListAsync();
    public Task<List<TaskItem>> GetAllTasksAsync() => _database.Table<TaskItem>().OrderBy(x => x.DueDate).ToListAsync();
    public async Task<int> AddTaskAsync(TaskItem task, int assignedByUserId)
    {
        await EnsureCanAssignTasksAsync(assignedByUserId);
        await _database.InsertAsync(task);
        await AddTaskAssignmentNotificationAsync(task.TaskId, task.AssignedUserId, task.Title);
        return task.TaskId;
    }

    public async Task AssignTaskAsync(int taskId, int userId, int assignedByUserId)
    {
        await EnsureCanAssignTasksAsync(assignedByUserId);
        var task = await _database.Table<TaskItem>().Where(x => x.TaskId == taskId).FirstOrDefaultAsync();
        await _database.ExecuteAsync("UPDATE TaskItem SET AssignedUserId = ? WHERE TaskId = ?", userId, taskId);
        await AddTaskAssignmentNotificationAsync(taskId, userId, task?.Title ?? $"Task #{taskId}");
    }

    private Task AddTaskAssignmentNotificationAsync(int taskId, int userId, string taskTitle)
    {
        return _database.InsertAsync(new Notification
        {
            UserId = userId,
            Title = "Task assigned",
            Message = $"The task '{taskTitle}' has been assigned to you."
        });
    }

    private async Task EnsureCanAssignTasksAsync(int userId)
    {
        var user = await GetUserAsync(userId);
        if (user is not { Role: UserRole.ProjectManager or UserRole.TeamLead })
        {
            throw new UnauthorizedAccessException("Only project managers and team leads can assign tasks.");
        }
    }
    public async Task<List<Project>> GetProjectsAsync(int userId)
    {
        var projects = await _database.Table<Project>().ToListAsync();
        var memberProjectIds = (await _database.Table<ProjectMember>().Where(x => x.UserId == userId).ToListAsync())
            .Select(x => x.ProjectId)
            .ToHashSet();
        return projects
            .Where(x => x.ProjectManagerId == userId || memberProjectIds.Contains(x.ProjectId))
            .OrderBy(x => x.Name)
            .ToList();
    }
    public Task<List<ProjectMember>> GetProjectMembersAsync(int projectId) => _database.Table<ProjectMember>().Where(x => x.ProjectId == projectId).ToListAsync();
    public Task<List<Announcement>> GetAnnouncementsAsync() => _database.Table<Announcement>().OrderByDescending(x => x.PublishDate).ToListAsync();
    public Task<List<Notification>> GetNotificationsAsync(int userId) => _database.Table<Notification>().Where(x => x.UserId == userId).OrderByDescending(x => x.NotificationId).ToListAsync();
    public Task<List<TaskForgeDocument>> GetDocumentsAsync(int userId) => _database.Table<TaskForgeDocument>().OrderByDescending(x => x.UploadedDate).ToListAsync();
    public Task UpdateTaskStatusAsync(int taskId, TaskItemStatus status) => _database.ExecuteAsync("UPDATE TaskItem SET Status = ? WHERE TaskId = ?", (int)status, taskId);
    public Task MarkNotificationReadAsync(int notificationId) => _database.ExecuteAsync("UPDATE Notification SET IsRead = 1 WHERE NotificationId = ?", notificationId);
    public Task UpdateUserAsync(User user) => _database.UpdateAsync(user);

    private async Task SeedAsync()
    {
        await _database.InsertAllAsync(new[]
        {
            new User { UserId = 1, Email = "admin@taskforge.app", DisplayName = "System Administrator", Department = "IT", JobTitle = "Administrator", Role = UserRole.Administrator },
            new User { UserId = 2, Email = "ali@taskforge.app", DisplayName = "Ali Patel", Department = "Engineering", JobTitle = "Project Manager", Role = UserRole.ProjectManager },
            new User { UserId = 3, Email = "mayank@taskforge.app", DisplayName = "Mayank Sharma", Department = "Engineering", JobTitle = "Team Lead", Role = UserRole.TeamLead },
            new User { UserId = 4, Email = "akshit@taskforge.app", DisplayName = "Akshit Kumar", Department = "Engineering", JobTitle = "Software Engineer", Role = UserRole.Employee }
        });
        await _database.InsertAsync(new Project { ProjectId = 1, Name = "TaskForge Development", Description = "Internal employee productivity dashboard", ProjectManagerId = 2, Status = ProjectStatus.Active, TargetCompletionDate = DateTime.UtcNow.AddDays(60) });
        await _database.InsertAllAsync(new[]
        {
            new TaskItem { TaskId = 1, Title = "Design database schema", Description = "Create entity relationship diagram", Priority = TaskPriority.High, Status = TaskItemStatus.Completed, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(-5) },
            new TaskItem { TaskId = 2, Title = "Implement authentication", Description = "Set up mock role-based authentication", Priority = TaskPriority.Critical, Status = TaskItemStatus.InProgress, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(5) },
            new TaskItem { TaskId = 3, Title = "Create UI mockups", Description = "Design the main dashboard pages", Priority = TaskPriority.Medium, Status = TaskItemStatus.NotStarted, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(10) }
        });
        await _database.InsertAllAsync(new[]
        {
            new ProjectMember { ProjectId = 1, UserId = 3, Role = "Team Lead" },
            new ProjectMember { ProjectId = 1, UserId = 4, Role = "Developer" }
        });
        await _database.InsertAsync(new Announcement { Title = "Welcome to TaskForge", Content = "Manage tasks, projects, documents, and team coordination in one place." });
        await _database.InsertAsync(new Notification { UserId = 4, Title = "New task assigned", Message = "Implement authentication was assigned to you." });
        await _database.InsertAsync(new TaskForgeDocument { Title = "Project brief", Category = "Planning", OriginalFileName = "project-brief.pdf", ContentType = "application/pdf", FilePath = "project-brief.pdf", FileSize = 128000, UploadedByUserId = 4, ProjectId = 1 });
    }
}