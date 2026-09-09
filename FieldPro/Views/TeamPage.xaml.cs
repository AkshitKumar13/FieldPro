using TaskForge.Models;
using TaskForge.Services;
using System.Collections.ObjectModel;
namespace TaskForge.Views;
public partial class TeamPage : ContentPage
{
    private readonly ITaskForgeDataService _data; public ObservableCollection<User> Items { get; } = new();
    public TeamPage(ITaskForgeDataService data) { InitializeComponent(); _data = data; BindingContext = this; }
    protected override async void OnAppearing() { base.OnAppearing(); Items.Clear(); foreach (var item in await _data.GetUsersAsync()) Items.Add(item); }
}