using TaskForge.Models;
using TaskForge.Services;
using System.Collections.ObjectModel;
using TaskItemStatus = TaskForge.Models.TaskStatus;
namespace TaskForge.Views;
public partial class TasksPage : ContentPage
{
    private readonly ITaskForgeDataService _data; private readonly IAppSession _session;
    public ObservableCollection<TaskItem> Items { get; } = new();
    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<Project> Projects { get; } = new();
    public IReadOnlyList<TaskItemStatus> Statuses { get; } = Enum.GetValues<TaskItemStatus>();
    public string NewTitle { get; set; } = string.Empty;
    public string NewDescription { get; set; } = string.Empty;
    public User? SelectedAssignee { get; set; }
    public Project? SelectedProject { get; set; }
    public bool CanAssignTasks => _session.CurrentUser is { Role: UserRole.ProjectManager or UserRole.TeamLead };
    public TasksPage(ITaskForgeDataService data, IAppSession session) { InitializeComponent(); _data = data; _session = session; BindingContext = this; }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        OnPropertyChanged(nameof(CanAssignTasks));
        Users.Clear();
        foreach (var user in await _data.GetUsersAsync()) Users.Add(user);
        Projects.Clear();
        foreach (var project in await _data.GetProjectsAsync(_session.CurrentUser?.UserId ?? 0)) Projects.Add(project);
        await ReloadTasksAsync();
    }

    private async Task ReloadTasksAsync()
    {
        Items.Clear();
        var user = _session.CurrentUser;
        var tasks = user is { Role: UserRole.Administrator or UserRole.ProjectManager or UserRole.TeamLead }
            ? await _data.GetAllTasksAsync()
            : await _data.GetTasksAsync(user?.UserId ?? 0);
        foreach (var item in tasks) Items.Add(item);
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        if (!CanAssignTasks)
        {
            await DisplayAlertAsync("Permission denied", "Only project managers and team leads can assign tasks.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewTitle) || SelectedAssignee == null || SelectedProject == null)
        {
            await DisplayAlertAsync("Missing information", "Enter a title, assignee, and project.", "OK");
            return;
        }

        await _data.AddTaskAsync(new TaskItem
        {
            Title = NewTitle.Trim(),
            Description = NewDescription.Trim(),
            AssignedUserId = SelectedAssignee.UserId,
            ProjectId = SelectedProject.ProjectId,
            DueDate = DateTime.UtcNow.AddDays(7)
        }, _session.CurrentUser!.UserId);
        NewTitle = string.Empty;
        NewDescription = string.Empty;
        OnPropertyChanged(nameof(NewTitle));
        OnPropertyChanged(nameof(NewDescription));
        await ReloadTasksAsync();
    }

    private async void OnStatusChanged(object sender, EventArgs e) { if (sender is Picker { BindingContext: TaskItem task }) await _data.UpdateTaskStatusAsync(task.TaskId, task.Status); }
    private async void OnAssigneeChanged(object sender, EventArgs e)
    {
        if (!CanAssignTasks)
        {
            await DisplayAlertAsync("Permission denied", "Only project managers and team leads can assign tasks.", "OK");
            return;
        }

        if (sender is Picker { BindingContext: TaskItem task, SelectedItem: User user })
        {
            await _data.AssignTaskAsync(task.TaskId, user.UserId, _session.CurrentUser!.UserId);
            task.AssignedUserId = user.UserId;
        }
    }
}