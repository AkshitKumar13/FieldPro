using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

using TaskForge.Services;
using TaskForge.ViewModels;

namespace TaskForge.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();

        // Resolve viewmodel from DI and set as BindingContext (Shell creates page via XAML)
        var services = App.Current?.Handler?.MauiContext?.Services;
        var vm = services?.GetService<DashboardViewModel>();
        if (vm != null)
            BindingContext = vm;
    }

    private async void OnOpenTasksTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Tasks");
    }

    private async void OnPendingTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Tasks");
    }

    private async void OnCompletedTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Tasks");
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
        var appSession = services?.GetService<IAppSession>();

        // Enable flyout for authenticated area
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);

        if (appSession == null || !appSession.IsAuthenticated)
        {
            // If not authenticated, return to login
            await Shell.Current.GoToAsync("//Login");
            return;
        }

        // Refresh dashboard counts every time the page appears
        if (BindingContext is DashboardViewModel dashboardVm)
        {
            await dashboardVm.LoadCountsAsync();
        }
    }
}