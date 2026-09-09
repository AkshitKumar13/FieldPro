using ContosoDashboard.Models;
using ContosoTaskStatus = ContosoDashboard.Models.TaskStatus;

namespace ContosoDashboard.Services;

public interface IContosoDataService
{
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserAsync(int id);
    Task<List<ContosoTask>> GetTasksAsync(int userId);
    Task<List<ContosoTask>> GetAllTasksAsync();
    Task<int> AddTaskAsync(ContosoTask task);
    Task AssignTaskAsync(int taskId, int userId);
    Task<List<Project>> GetProjectsAsync(int userId);
    Task<List<ProjectMember>> GetProjectMembersAsync(int projectId);
    Task<List<Announcement>> GetAnnouncementsAsync();
    Task<List<Notification>> GetNotificationsAsync(int userId);
    Task<List<ContosoDocument>> GetDocumentsAsync(int userId);
    Task UpdateTaskStatusAsync(int taskId, ContosoTaskStatus status);
    Task MarkNotificationReadAsync(int notificationId);
    Task UpdateUserAsync(User user);
}