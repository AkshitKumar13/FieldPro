using CommunityToolkit.Mvvm.ComponentModel;
using TaskForge.Services;
using System.Threading.Tasks;

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

    public async Task LoadCountsAsync()
    {
        var all = await _data.GetTasksAsync(_session.CurrentUser?.UserId ?? 0);

        if (all == null)
        {
            OpenCount = 0;
            PendingCount = 0;
            CompletedCount = 0;
            return;
        }

        CompletedCount = all.Count(w => w.Status == Models.TaskStatus.Completed);
        PendingCount = all.Count(w => w.Status == Models.TaskStatus.NotStarted);
        OpenCount = all.Count(w => w.Status == Models.TaskStatus.InProgress);
    }
}
