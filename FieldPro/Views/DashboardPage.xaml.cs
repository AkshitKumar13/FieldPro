using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace FieldPro.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();

        // Resolve viewmodel from DI and set as BindingContext (Shell creates page via XAML)
        var services = App.Current?.Handler?.MauiContext?.Services;
        var vm = services?.GetService<FieldPro.ViewModels.DashboardViewModel>();
        if (vm != null)
            BindingContext = vm;
    }

    private async void OnWorkOrdersClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("WorkOrders");
    }

    private async void OnOpenWorkOrdersTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("WorkOrders", new Dictionary<string, object>
        {
            ["StatusFilter"] = "InProgress"
        });
    }

    private async void OnPendingTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("WorkOrders", new Dictionary<string, object>
        {
            ["StatusFilter"] = "Pending"
        });
    }

    private async void OnCompletedTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("WorkOrders", new Dictionary<string, object>
        {
            ["StatusFilter"] = "Completed"
        });
    }

    private async void OnSystemStatusTapped(object sender, EventArgs e)
    {
        await DisplayAlertAsync("System Status", "The system is online.", "OK");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Resolve app session from DI at runtime
        var services = App.Current?.Handler?.MauiContext?.Services;
        var appSession = services?.GetService<FieldPro.Services.IAppSession>();

        // Enable flyout for authenticated area
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);

        if (appSession == null || !appSession.IsAuthenticated)
        {
            // If not authenticated, return to login
            await Shell.Current.GoToAsync("//Login");
            return;
        }

        // Refresh dashboard counts every time the page appears
        if (BindingContext is FieldPro.ViewModels.DashboardViewModel dashboardVm)
        {
            await dashboardVm.LoadCountsAsync();
        }
    }
}