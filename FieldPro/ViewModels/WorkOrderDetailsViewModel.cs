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
    private string? photoPath;
    public bool IsStartEnabled =>
    !IsBusy &&
    WorkOrder?.Status?.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true;

    public bool IsCompleteEnabled =>
        !IsBusy &&
        WorkOrder?.Status?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true;
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

            // Check if camera is available
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Camera Unavailable",
                    "Camera is not available on this device.",
                    "OK");

                return;
            }

            // Open camera
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            // User cancelled camera
            if (photo == null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Photo Required",
                    "Please capture a photo before completing the work order.",
                    "OK");

                return;
            }

            // Create file name
            var fileName =
                $"WorkOrder_{WorkOrder.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";

            // Save inside app's local storage
            var destinationPath = Path.Combine(
                FileSystem.AppDataDirectory,
                fileName);

            await using var sourceStream =
                await photo.OpenReadAsync();

            await using var destinationStream =
                File.Create(destinationPath);

            await sourceStream.CopyToAsync(destinationStream);

            // Keep path for later use
            PhotoPath = destinationPath;

            // Photo successfully captured
            // NOW mark the work order as completed
            WorkOrder.Status = "Completed";

            // If your WorkOrder model has a photo property,
            // you can save the path here:
            //
            // WorkOrder.PhotoPath = destinationPath;

            await _workOrderService.SaveWorkOrderAsync(WorkOrder);

            await Shell.Current.DisplayAlertAsync(
                "Work Completed",
                $"Work order #{WorkOrder.Id} has been completed.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                $"Unable to complete work order.\n\n{ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}