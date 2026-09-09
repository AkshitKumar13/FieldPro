using ContosoDashboard.Services;
namespace ContosoDashboard.Views;
public partial class ProfilePage : ContentPage
{
    private readonly IContosoDataService _data; private readonly IAppSession _session; public object? CurrentUser => _session.CurrentUser;
    public ProfilePage(IContosoDataService data, IAppSession session) { InitializeComponent(); _data = data; _session = session; BindingContext = this; }
    private async void OnSaveClicked(object sender, EventArgs e) { if (_session.CurrentUser is not null) await _data.UpdateUserAsync(_session.CurrentUser); await DisplayAlertAsync("Profile", "Profile saved locally.", "OK"); }
}