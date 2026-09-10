using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskForge.Models;
using TaskForge.Services;
using System.Collections.ObjectModel;

namespace TaskForge.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IAppSession _appSession;

    [ObservableProperty]
    private User? selectedUser;

    public ObservableCollection<User> Users { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool HasError =>
        !string.IsNullOrWhiteSpace(ErrorMessage);

    public LoginViewModel(IAuthService authService, IAppSession appSession, TaskForge.Data.TaskForgeDatabase database)
    {
        _authService = authService;
        _appSession = appSession;
        _ = LoadUsersAsync(database);
    }

    private async Task LoadUsersAsync(TaskForge.Data.TaskForgeDatabase database)
    {
        await database.InitializeAsync();
        foreach (var user in await database.GetUsersAsync()) Users.Add(user);
        SelectedUser = Users.FirstOrDefault();
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedUser is null)
            {
                ErrorMessage = "Please choose a user.";
                return;
            }

            var user = await _authService.LoginAsync(SelectedUser.Email);

            if (user is null)
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }

            // IMPORTANT: Set session as authenticated
            _appSession.Login(user);

            // Navigate to Main -> Dashboard
            await Shell.Current.GoToAsync("//Main/Dashboard");
        }
        finally
        {
            IsBusy = false;
        }
    }
}