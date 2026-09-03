using FieldPro.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FieldPro.Views;

[QueryProperty(nameof(StatusFilter), "StatusFilter")]
public partial class WorkOrdersPage : ContentPage
{
    private string? statusFilter;

    public string? StatusFilter
    {
        get => statusFilter;
        set => statusFilter = value;
    }

    public WorkOrdersPage(WorkOrdersViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is WorkOrdersViewModel viewModel)
        {
            await viewModel.LoadWorkOrdersFilteredAsync(StatusFilter);
        }
    }
}
