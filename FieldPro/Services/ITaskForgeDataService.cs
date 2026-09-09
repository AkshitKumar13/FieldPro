using TaskForge.Models;
using TaskItemStatus = TaskForge.Models.TaskStatus;

namespace TaskForge.Services;

public interface ITaskForgeDataService
{
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserAsync(int id);
    Task<List<TaskItem>> GetTasksAsync(int userId);
    Task<List<TaskItem>> GetAllTasksAsync();
    Task<int> AddTaskAsync(TaskItem task, int assignedByUserId);
    Task AssignTaskAsync(int taskId, int userId, int assignedByUserId);
    Task<List<Project>> GetProjectsAsync(int userId);
    Task<List<ProjectMember>> GetProjectMembersAsync(int projectId);
    Task<List<Announcement>> GetAnnouncementsAsync();
    Task<List<Notification>> GetNotificationsAsync(int userId);
    Task<List<TaskForgeDocument>> GetDocumentsAsync(int userId);
    Task UpdateTaskStatusAsync(int taskId, TaskItemStatus status);
    Task MarkNotificationReadAsync(int notificationId);
    Task UpdateUserAsync(User user);
}