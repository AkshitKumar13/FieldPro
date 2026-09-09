using ContosoDashboard.Models;
using ContosoDashboard.Services;
using System.Collections.ObjectModel;
namespace ContosoDashboard.Views;
public partial class TeamPage : ContentPage
{
    private readonly IContosoDataService _data; public ObservableCollection<User> Items { get; } = new();
    public TeamPage(IContosoDataService data) { InitializeComponent(); _data = data; BindingContext = this; }
    protected override async void OnAppearing() { base.OnAppearing(); Items.Clear(); foreach (var item in await _data.GetUsersAsync()) Items.Add(item); }
}