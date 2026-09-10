using System;
using Microsoft.Extensions.DependencyInjection;

namespace FieldPro;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            "WorkOrders",
            typeof(Views.WorkOrdersPage));

        Routing.RegisterRoute(
            "WorkOrderDetails",
            typeof(Views.WorkOrderDetailsPage));
    }

    private async void OnReseedClicked(object sender, EventArgs e)
    {
        try
        {
#if DEBUG
            var services = App.Current?.Handler?.MauiContext?.Services;
            var seeder = services?.GetService<FieldPro.Data.DatabaseSeeder>();
            var database = services?.GetService<FieldPro.Data.FieldProDatabase>();

            if (seeder == null || database == null)
            {
                await Shell.Current.DisplayAlertAsync("Reseed", "Seeder or database not available.", "OK");
                return;
            }

            await database.InitializeAsync();
            await seeder.ForceSeedAsync(database);

            await Shell.Current.DisplayAlertAsync("Reseed", "Database reseeded from seed.json.", "OK");

            // Navigate to Dashboard so counts will refresh
            await Shell.Current.GoToAsync("//Dashboard");
#else
            await Shell.Current.DisplayAlert("Reseed", "Reseed is only available in DEBUG builds.", "OK");
#endif
        }
        catch (Exception)
        {
            // ignore
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        try
        {
            // Resolve services from the MAUI service provider
            var services = App.Current?.Handler?.MauiContext?.Services;
            var appSession = services?.GetService<FieldPro.Services.IAppSession>();
            var authService = services?.GetService<FieldPro.Services.IAuthService>();

            if (authService != null)
            {
                await authService.LogoutAsync();
            }
            appSession?.Logout();

            // Navigate to the login page (clear stack)
            await Shell.Current.GoToAsync("//Login");
        }
        catch
        {
            // Ignore errors here; logout is best-effort
        }
    }
}