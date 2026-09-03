using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldPro.Services;

namespace FieldPro.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IAppSession _appSession;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool HasError =>
        !string.IsNullOrWhiteSpace(ErrorMessage);

    public LoginViewModel(IAuthService authService, IAppSession appSession)
    {
        _authService = authService;
        _appSession = appSession;
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

            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter email and password.";
                return;
            }

            var success = await _authService.LoginAsync(
                Email,
                Password);

            if (!success)
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }

            _appSession.Login();

            await Shell.Current.GoToAsync("//Main");
        }
        finally
        {
            IsBusy = false;
        }
    }
}