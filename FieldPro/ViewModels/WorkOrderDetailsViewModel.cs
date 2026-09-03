using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldPro.Models;
using FieldPro.Services;

namespace FieldPro.ViewModels;

[QueryProperty(nameof(WorkOrder), "WorkOrder")]
public partial class WorkOrderDetailsViewModel : ObservableObject
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrderDetailsViewModel(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    [ObservableProperty]
    private WorkOrder? workOrder;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isStartEnabled;

    [ObservableProperty]
    private bool isCompleteEnabled;

    [RelayCommand]
    private async Task StartWorkAsync()
    {
        if (WorkOrder == null || IsBusy)
            return;

        try
        {
            IsBusy = true;

            WorkOrder.Status = "In Progress";

            await _workOrderService.SaveWorkOrderAsync(WorkOrder);

            await Shell.Current.DisplayAlertAsync(
                "Work Started",
                $"Work order #{WorkOrder.Id} has been started.",
                "OK");

            UpdateButtonStates();

            // Navigate back to the previous page so the work orders list can refresh
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CompleteWorkAsync()
    {
        if (WorkOrder == null || IsBusy)
            return;

        try
        {
            IsBusy = true;

            WorkOrder.Status = "Completed";

            await _workOrderService.SaveWorkOrderAsync(WorkOrder);

            await Shell.Current.DisplayAlertAsync(
                "Work Completed",
                $"Work order #{WorkOrder.Id} has been completed.",
                "OK");

            UpdateButtonStates();

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnWorkOrderChanged(WorkOrder? value)
    {
        UpdateButtonStates();
    }

    partial void OnIsBusyChanged(bool value)
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (WorkOrder == null)
        {
            IsStartEnabled = false;
            IsCompleteEnabled = false;
            return;
        }

        // Start only when pending
        IsStartEnabled = !IsBusy && string.Equals(WorkOrder.Status, "Pending", StringComparison.OrdinalIgnoreCase);

        // Complete only when In Progress
        IsCompleteEnabled = !IsBusy && string.Equals(WorkOrder.Status, "In Progress", StringComparison.OrdinalIgnoreCase);
    }
}