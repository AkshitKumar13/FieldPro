using CommunityToolkit.Maui;
using TaskForge.Services;
using TaskForge.Data;
using TaskForge.ViewModels;
using TaskForge.Views;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

// Note: run DB initialization/seeding asynchronously to avoid blocking the UI thread on Android.

namespace TaskForge
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
#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("TaskForgeAndroid", (handler, _) =>
                handler.PlatformView.SetPadding(12, 0, 12, 0));
#elif IOS
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("TaskForgeIos", (handler, _) =>
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.RoundedRect);
#endif
#if DEBUG
            builder.Logging.AddDebug();
#endif
            //services
            builder.Services.AddSingleton<TaskForgeDatabase>();
            builder.Services.AddSingleton<ITaskForgeDataService>(sp => sp.GetRequiredService<TaskForgeDatabase>());
            builder.Services.AddSingleton<IAppSession, AppSession>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<AppStateService>();
            builder.Services.AddSingleton<IDeviceCapabilitiesService, DeviceCapabilitiesService>();

            //pages/models
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<TasksViewModel>();
            builder.Services.AddTransient<NotificationsViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<DeviceCapabilitiesViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<TasksPage>();
            builder.Services.AddTransient<ProjectsPage>();
            builder.Services.AddTransient<TeamPage>();
            builder.Services.AddTransient<NotificationsPage>();
            builder.Services.AddTransient<DocumentsPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<DeviceCapabilitiesPage>();
            var mauiApp = builder.Build();

            // Start DB initialization/seeding on a background task so we don't block startup (avoids Android deadlocks).
            _ = Task.Run(async () =>
            {
                try
                {
                    var database = mauiApp.Services.GetService<TaskForgeDatabase>();

                    if (database != null)
                    {
                        await database.InitializeAsync();

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