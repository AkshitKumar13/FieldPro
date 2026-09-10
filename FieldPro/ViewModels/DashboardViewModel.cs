using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskForge.Services;
using System.Threading.Tasks;
using TaskForge.Models;

namespace TaskForge.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ITaskForgeDataService _data;
    private readonly IAppSession _session;

    public string UserName => _session.CurrentUser?.DisplayName ?? "User";
    public string RoleName => _session.CurrentUser?.Role.ToString() ?? string.Empty;

    public DashboardViewModel(ITaskForgeDataService data, IAppSession session)
    {
        _data = data;
        _session = session;
    }

    [ObservableProperty]
    private int openCount;

    [ObservableProperty]
    private int pendingCount;

    [ObservableProperty]
    private int completedCount;

    [ObservableProperty]
    private string systemStatus = "Online";

    [RelayCommand]
    private Task OpenTasksAsync() => Shell.Current.GoToAsync("//Tasks");

    [RelayCommand]
    private Task OpenStatusAsync() => Shell.Current.DisplayAlertAsync("System Status", "The system is online.", "OK");

    [RelayCommand]
    private Task OpenDocumentsAsync() => Shell.Current.GoToAsync("//Documents");

    public async Task LoadCountsAsync()
    {
        var user = _session.CurrentUser;
        var all = user is { Role: UserRole.Administrator or UserRole.ProjectManager or UserRole.TeamLead }
            ? await _data.GetAllTasksAsync()
            : await _data.GetTasksAsync(user?.UserId ?? 0);
        all ??= new List<TaskItem>();

        CompletedCount = all.Count(w => w.Status == Models.TaskStatus.Completed);
        PendingCount = all.Count(w => w.Status == Models.TaskStatus.NotStarted);
        OpenCount = all.Count(w => w.Status == Models.TaskStatus.InProgress);
    }
}
