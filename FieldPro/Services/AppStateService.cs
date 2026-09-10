using Microsoft.Maui.Storage;

namespace TaskForge.Services;

public sealed class AppStateService
{
    private const string LastRouteKey = "taskforge.last-route";

    public bool IsInBackground { get; private set; }

    public string LastRoute
    {
        get => Preferences.Default.Get(LastRouteKey, "//Login");
        private set => Preferences.Default.Set(LastRouteKey, value);
    }

    public void MarkStarted() => IsInBackground = false;

    public void MarkSleeping() => IsInBackground = true;

    public void MarkResumed() => IsInBackground = false;

    public void SaveRoute(string route)
    {
        if (!string.IsNullOrWhiteSpace(route))
        {
            LastRoute = route;
        }
    }
}
