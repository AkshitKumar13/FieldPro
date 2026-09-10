using TaskForge.Services;

namespace TaskForge
{
    public partial class App : Application
    {
        private readonly AppStateService _appState;

        public App(AppStateService appState)
        {
            _appState = appState;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override void OnStart() => _appState.MarkStarted();

        protected override void OnSleep()
        {
            _appState.MarkSleeping();
            if (MainPage is Shell shell)
            {
                _appState.SaveRoute(shell.CurrentState.Location.OriginalString);
            }
        }

        protected override void OnResume() => _appState.MarkResumed();
    }
}
