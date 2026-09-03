using CommunityToolkit.Maui;
using FieldPro.Services;
using FieldPro.Data;
using FieldPro.ViewModels;
using FieldPro.Views;
using Microsoft.Extensions.Logging;

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
            return builder.Build();
        }
    }
}