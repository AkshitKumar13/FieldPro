using CommunityToolkit.Mvvm.ComponentModel;
using FieldPro.Services;
using System.Threading.Tasks;

namespace FieldPro.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IWorkOrderService _workOrderService;

    public DashboardViewModel(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
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
        var all = await _workOrderService.GetWorkOrdersAsync();

        if (all == null)
        {
            OpenCount = 0;
            PendingCount = 0;
            CompletedCount = 0;
            return;
        }

        CompletedCount = all.Count(w => string.Equals(w.Status, "Completed", StringComparison.OrdinalIgnoreCase));
        PendingCount = all.Count(w => string.Equals(w.Status, "Pending", StringComparison.OrdinalIgnoreCase));
        // Open = not completed
        OpenCount = all.Count(w => !string.Equals(w.Status, "Completed", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(w.Status, "Pending", StringComparison.OrdinalIgnoreCase));
    }
}
