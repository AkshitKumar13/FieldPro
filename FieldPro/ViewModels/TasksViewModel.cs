using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskForge.Models;
using TaskForge.Services;
using TaskItemStatus = TaskForge.Models.TaskStatus;

namespace TaskForge.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    private readonly ITaskForgeDataService _data;
    private readonly IAppSession _session;

    public ObservableCollection<TaskItem> Items { get; } = new();
    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<Project> Projects { get; } = new();
    public IReadOnlyList<TaskItemStatus> Statuses { get; } = Enum.GetValues<TaskItemStatus>();

    [ObservableProperty] private string? statusFilter;
    [ObservableProperty] private string newTitle = string.Empty;
    [ObservableProperty] private string newDescription = string.Empty;
    [ObservableProperty] private User? selectedAssignee;
    [ObservableProperty] private Project? selectedProject;
    [ObservableProperty] private bool isBusy;

    public bool CanAssignTasks => _session.CurrentUser is { Role: UserRole.ProjectManager or UserRole.TeamLead or UserRole.Administrator };

    public TasksViewModel(ITaskForgeDataService data, IAppSession session)
    {
        _data = data;
        _session = session;
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Users.Clear();
            foreach (var user in await _data.GetUsersAsync()) Users.Add(user);
            Projects.Clear();
            foreach (var project in await _data.GetProjectsAsync(_session.CurrentUser?.UserId ?? 0)) Projects.Add(project);
            await ReloadAsync();
            OnPropertyChanged(nameof(CanAssignTasks));
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task AddTaskAsync()
    {
        if (!CanAssignTasks || SelectedAssignee is null || SelectedProject is null || string.IsNullOrWhiteSpace(NewTitle)) return;
        await _data.AddTaskAsync(new TaskItem
        {
            Title = NewTitle.Trim(), Description = NewDescription.Trim(),
            AssignedUserId = SelectedAssignee.UserId, ProjectId = SelectedProject.ProjectId,
            DueDate = DateTime.UtcNow.AddDays(7)
        }, _session.CurrentUser!.UserId);
        NewTitle = string.Empty;
        NewDescription = string.Empty;
        await ReloadAsync();
    }

    [RelayCommand]
    private Task UpdateStatusAsync(TaskItem task) => _data.UpdateTaskStatusAsync(task.TaskId, task.Status);

    [RelayCommand]
    private async Task ReassignAsync(TaskAssignment assignment)
    {
        if (!CanAssignTasks || assignment.Task is null || assignment.User is null) return;
        await _data.AssignTaskAsync(assignment.Task.TaskId, assignment.User.UserId, _session.CurrentUser!.UserId);
        assignment.Task.AssignedUserId = assignment.User.UserId;
    }

    private async Task ReloadAsync()
    {
        Items.Clear();
        var user = _session.CurrentUser;
        var tasks = user is { Role: UserRole.Administrator or UserRole.ProjectManager or UserRole.TeamLead }
            ? await _data.GetAllTasksAsync() : await _data.GetTasksAsync(user?.UserId ?? 0);
        if (Enum.TryParse<TaskItemStatus>(StatusFilter, true, out var filter))
            tasks = tasks.Where(task => task.Status == filter).ToList();
        foreach (var task in tasks)
        {
            task.AssignedUserName = Users.FirstOrDefault(user => user.UserId == task.AssignedUserId)?.DisplayName
                ?? "Unassigned";
            Items.Add(task);
        }
    }
}

public sealed record TaskAssignment(TaskItem? Task, User? User);
