using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldPro.Models;
using FieldPro.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace FieldPro.ViewModels;

public partial class WorkOrdersViewModel : ObservableObject
{
    private readonly IWorkOrderService _workOrderService;

    public ObservableCollection<WorkOrder> WorkOrders { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool HasError =>
        !string.IsNullOrWhiteSpace(ErrorMessage);

    public WorkOrdersViewModel(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    [RelayCommand]
    private async Task LoadWorkOrdersAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var workOrders =
                await _workOrderService.GetWorkOrdersAsync();

            WorkOrders.Clear();

            foreach (var workOrder in workOrders)
            {
                WorkOrders.Add(workOrder);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Unable to load work orders.";
        }
        finally
        {
            IsBusy = false;
        }

    }

    public async Task LoadWorkOrdersFilteredAsync(string? statusFilter)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var workOrders = await _workOrderService.GetWorkOrdersAsync();

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                if (string.Equals(statusFilter, "Open", StringComparison.OrdinalIgnoreCase))
                {
                    workOrders = workOrders.Where(w => !string.Equals(w.Status, "Completed", StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    workOrders = workOrders.Where(w => string.Equals(w.Status, statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            WorkOrders.Clear();

            foreach (var workOrder in workOrders)
            {
                WorkOrders.Add(workOrder);
            }
        }
        catch
        {
            ErrorMessage = "Unable to load work orders.";
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    private async Task WorkOrderSelectedAsync(WorkOrder? workOrder)
    {
        if (workOrder == null)
            return;

        await Shell.Current.GoToAsync(
            "WorkOrderDetails",
            new Dictionary<string, object>
            {
                ["WorkOrder"] = workOrder
            });
    }
}