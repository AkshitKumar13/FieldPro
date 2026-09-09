using SQLite;

namespace ContosoDashboard.Models;

public enum UserRole { Employee, TeamLead, ProjectManager, Administrator }
public enum AvailabilityStatus { Available, Busy, InMeeting, OutOfOffice }
public enum TaskPriority { Low, Medium, High, Critical }
public enum TaskStatus { NotStarted, InProgress, Completed }
public enum ProjectStatus { Planning, Active, OnHold, Completed }

public class User
{
    [PrimaryKey, AutoIncrement] public int UserId { get; set; }
    [Unique, MaxLength(255)] public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public AvailabilityStatus AvailabilityStatus { get; set; } = AvailabilityStatus.Available;
    public bool InAppNotificationsEnabled { get; set; } = true;
}

public class ContosoTask
{
    [PrimaryKey, AutoIncrement] public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
    public DateTime? DueDate { get; set; }
    public int AssignedUserId { get; set; }
    public int ProjectId { get; set; }
}

public class Project
{
    [PrimaryKey, AutoIncrement] public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProjectManagerId { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public DateTime TargetCompletionDate { get; set; }
}

public class ProjectMember
{
    [PrimaryKey, AutoIncrement] public int ProjectMemberId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class Announcement
{
    [PrimaryKey, AutoIncrement] public int AnnouncementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
}

public class Notification
{
    [PrimaryKey, AutoIncrement] public int NotificationId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}

public class ContosoDocument
{
    [PrimaryKey, AutoIncrement] public int DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int UploadedByUserId { get; set; }
    public int? ProjectId { get; set; }
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}