using TaskForge.ViewModels;

namespace TaskForge.Views;

public partial class DeviceCapabilitiesPage : ContentPage
{
    private readonly DeviceCapabilitiesViewModel _viewModel;

    public DeviceCapabilitiesPage(DeviceCapabilitiesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        _viewModel.StopMonitoring();
        base.OnDisappearing();
    }
}
