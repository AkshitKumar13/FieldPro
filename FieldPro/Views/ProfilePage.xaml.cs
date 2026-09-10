using TaskForge.ViewModels;
namespace TaskForge.Views;
public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel) { InitializeComponent(); BindingContext = viewModel; }
}