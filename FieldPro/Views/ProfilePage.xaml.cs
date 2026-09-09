using TaskForge.Services;
namespace TaskForge.Views;
public partial class ProfilePage : ContentPage
{
    private readonly ITaskForgeDataService _data; private readonly IAppSession _session; public object? CurrentUser => _session.CurrentUser;
    public ProfilePage(ITaskForgeDataService data, IAppSession session) { InitializeComponent(); _data = data; _session = session; BindingContext = this; }
    private async void OnSaveClicked(object sender, EventArgs e) { if (_session.CurrentUser is not null) await _data.UpdateUserAsync(_session.CurrentUser); await DisplayAlertAsync("Profile", "Profile saved locally.", "OK"); }
}