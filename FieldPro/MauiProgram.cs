using CommunityToolkit.Maui;
using FieldPro.Services;
using FieldPro.Data;
using FieldPro.ViewModels;
using FieldPro.Views;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

// Note: run DB initialization/seeding asynchronously to avoid blocking the UI thread on Android.

namespace FieldPro
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            //services
            builder.Services.AddSingleton<FieldProDatabase>();
            builder.Services.AddPooledDbContextFactory<AuthDbContext>(options =>
                options.UseSqlite($"Data Source={AuthDbContext.GetDatabasePath()}"));
            builder.Services.AddSingleton<IAppSession, AppSession>();
            builder.Services.AddSingleton<DatabaseSeeder>();

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IWorkOrderService, WorkOrderService>();

            //pages/models
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<WorkOrdersViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<WorkOrdersPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<WorkOrderDetailsViewModel>();
            builder.Services.AddTransient<WorkOrderDetailsPage>();
            var mauiApp = builder.Build();

            // Start DB initialization/seeding on a background task so we don't block startup (avoids Android deadlocks).
            _ = Task.Run(async () =>
            {
                try
                {
                    var database = mauiApp.Services.GetService<FieldPro.Data.FieldProDatabase>();
                    var seeder = mauiApp.Services.GetService<FieldPro.Data.DatabaseSeeder>();

                    if (database != null)
                    {
                        await database.InitializeAsync();

                        var authService = mauiApp.Services.GetRequiredService<IAuthService>();
                        await authService.InitializeAsync();

                        if (seeder != null)
                        {
                            await seeder.SeedAsync(database);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        var logPath = Path.Combine(FileSystem.AppDataDirectory, "db_init_error.txt");
                        var text = $"[{DateTime.UtcNow:u}] Database initialization error: {ex}\n";
                        File.AppendAllText(logPath, text);
                        System.Diagnostics.Debug.WriteLine(text);
                    }
                    catch
                    {
                        // swallow - don't let logging throw
                    }
                }
            });

            return mauiApp;
        }
    }
}