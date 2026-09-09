using ContosoDashboard.Models;
using Microsoft.Maui.Storage;
using SQLite;
using ContosoTaskStatus = ContosoDashboard.Models.TaskStatus;

namespace ContosoDashboard.Data;

public sealed class ContosoDatabase : ContosoDashboard.Services.IContosoDataService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _initialized;

    public ContosoDatabase()
    {
        _database = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "contoso-dashboard.db3"));
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        await _database.CreateTableAsync<User>();
        await _database.CreateTableAsync<ContosoTask>();
        await _database.CreateTableAsync<Project>();
        await _database.CreateTableAsync<ProjectMember>();
        await _database.CreateTableAsync<Announcement>();
        await _database.CreateTableAsync<Notification>();
        await _database.CreateTableAsync<ContosoDocument>();
        if (await _database.Table<User>().CountAsync() == 0) await SeedAsync();
        _initialized = true;
    }

    public Task<List<User>> GetUsersAsync() => _database.Table<User>().OrderBy(x => x.DisplayName).ToListAsync();
    public Task<User?> GetUserAsync(int id) => _database.Table<User>().Where(x => x.UserId == id).FirstOrDefaultAsync();
    public Task<List<ContosoTask>> GetTasksAsync(int userId) => _database.Table<ContosoTask>().Where(x => x.AssignedUserId == userId).OrderBy(x => x.DueDate).ToListAsync();
    public Task<List<ContosoTask>> GetAllTasksAsync() => _database.Table<ContosoTask>().OrderBy(x => x.DueDate).ToListAsync();
    public async Task<int> AddTaskAsync(ContosoTask task, int assignedByUserId)
    {
        await EnsureCanAssignTasksAsync(assignedByUserId);
        await _database.InsertAsync(task);
        return task.TaskId;
    }

    public async Task AssignTaskAsync(int taskId, int userId, int assignedByUserId)
    {
        await EnsureCanAssignTasksAsync(assignedByUserId);
        await _database.ExecuteAsync("UPDATE ContosoTask SET AssignedUserId = ? WHERE TaskId = ?", userId, taskId);
        await _database.InsertAsync(new Notification
        {
            UserId = userId,
            Title = "Task assigned",
            Message = $"A task has been assigned to you (task #{taskId})."
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
    public Task<List<ContosoDocument>> GetDocumentsAsync(int userId) => _database.Table<ContosoDocument>().Where(x => x.UploadedByUserId == userId).OrderByDescending(x => x.UploadedDate).ToListAsync();
    public Task UpdateTaskStatusAsync(int taskId, ContosoTaskStatus status) => _database.ExecuteAsync("UPDATE ContosoTask SET Status = ? WHERE TaskId = ?", (int)status, taskId);
    public Task MarkNotificationReadAsync(int notificationId) => _database.ExecuteAsync("UPDATE Notification SET IsRead = 1 WHERE NotificationId = ?", notificationId);
    public Task UpdateUserAsync(User user) => _database.UpdateAsync(user);

    private async Task SeedAsync()
    {
        await _database.InsertAllAsync(new[]
        {
            new User { UserId = 1, Email = "admin@contoso.com", DisplayName = "System Administrator", Department = "IT", JobTitle = "Administrator", Role = UserRole.Administrator },
            new User { UserId = 2, Email = "camille.nicole@contoso.com", DisplayName = "Camille Nicole", Department = "Engineering", JobTitle = "Project Manager", Role = UserRole.ProjectManager },
            new User { UserId = 3, Email = "floris.kregel@contoso.com", DisplayName = "Floris Kregel", Department = "Engineering", JobTitle = "Team Lead", Role = UserRole.TeamLead },
            new User { UserId = 4, Email = "ni.kang@contoso.com", DisplayName = "Ni Kang", Department = "Engineering", JobTitle = "Software Engineer", Role = UserRole.Employee }
        });
        await _database.InsertAsync(new Project { ProjectId = 1, Name = "ContosoDashboard Development", Description = "Internal employee productivity dashboard", ProjectManagerId = 2, Status = ProjectStatus.Active, TargetCompletionDate = DateTime.UtcNow.AddDays(60) });
        await _database.InsertAllAsync(new[]
        {
            new ContosoTask { TaskId = 1, Title = "Design database schema", Description = "Create entity relationship diagram", Priority = TaskPriority.High, Status = ContosoTaskStatus.Completed, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(-5) },
            new ContosoTask { TaskId = 2, Title = "Implement authentication", Description = "Set up mock role-based authentication", Priority = TaskPriority.Critical, Status = ContosoTaskStatus.InProgress, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(5) },
            new ContosoTask { TaskId = 3, Title = "Create UI mockups", Description = "Design the main dashboard pages", Priority = TaskPriority.Medium, Status = ContosoTaskStatus.NotStarted, AssignedUserId = 4, ProjectId = 1, DueDate = DateTime.UtcNow.AddDays(10) }
        });
        await _database.InsertAllAsync(new[]
        {
            new ProjectMember { ProjectId = 1, UserId = 3, Role = "Team Lead" },
            new ProjectMember { ProjectId = 1, UserId = 4, Role = "Developer" }
        });
        await _database.InsertAsync(new Announcement { Title = "Welcome to ContosoDashboard", Content = "Manage tasks, projects, documents, and team coordination in one place." });
        await _database.InsertAsync(new Notification { UserId = 4, Title = "New task assigned", Message = "Implement authentication was assigned to you." });
        await _database.InsertAsync(new ContosoDocument { Title = "Project brief", Category = "Planning", OriginalFileName = "project-brief.pdf", ContentType = "application/pdf", FileSize = 128000, UploadedByUserId = 4, ProjectId = 1 });
    }
}