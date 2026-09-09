using CommunityToolkit.Mvvm.ComponentModel;

namespace FieldPro.Models;

public partial class WorkOrder : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string customerName = string.Empty;

    [ObservableProperty]
    private string address = string.Empty;

    [ObservableProperty]
    private string priority = "Medium";

    [ObservableProperty]
    private string status = "Pending";

    [ObservableProperty]
    private DateTime scheduledDate;

    [ObservableProperty]
    private string description = string.Empty;
}