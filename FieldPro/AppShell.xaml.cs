using System;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoDashboard;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

    }

    private async void OnReseedClicked(object sender, EventArgs e)
    {
        try
        {
#if DEBUG
            var services = App.Current?.Handler?.MauiContext?.Services;
            var database = services?.GetService<Data.ContosoDatabase>();

            if (database == null)
            {
                await Shell.Current.DisplayAlertAsync("Reseed", "Seeder or database not available.", "OK");
                return;
            }

            await database.InitializeAsync();
            await Shell.Current.DisplayAlertAsync("Database", "SQLite database initialized.", "OK");

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
            var appSession = services?.GetService<Services.IAppSession>();

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