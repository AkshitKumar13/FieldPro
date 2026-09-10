using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskForge.Models;
using TaskForge.Services;

namespace TaskForge.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly ITaskForgeDataService _data;
    private readonly IAppSession _session;

    public User? CurrentUser => _session.CurrentUser;

    public ProfileViewModel(ITaskForgeDataService data, IAppSession session)
    {
        _data = data;
        _session = session;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (CurrentUser is not null)
        {
            await _data.UpdateUserAsync(CurrentUser);
        }
        await Shell.Current.DisplayAlertAsync("Profile", "Profile saved locally.", "OK");
    }
}
