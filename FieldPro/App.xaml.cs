using Microsoft.Extensions.DependencyInjection;
using FieldPro.Data;

namespace FieldPro
{
    public partial class App : Application
    {
        public App(FieldProDatabase database, DatabaseSeeder seeder)
        {
            InitializeComponent();

            InitializeDatabaseAsync(database, seeder);
        }

        private static async void InitializeDatabaseAsync(
            FieldProDatabase database,
            DatabaseSeeder seeder)
        {
            await database.InitializeAsync();
            await seeder.SeedAsync(database);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
