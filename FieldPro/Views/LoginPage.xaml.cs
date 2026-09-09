using ContosoDashboard.ViewModels;

namespace ContosoDashboard.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Disable flyout while on login page to prevent bypass
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }
}