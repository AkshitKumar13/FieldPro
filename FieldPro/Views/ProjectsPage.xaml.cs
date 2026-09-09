using TaskForge.Models;
using TaskForge.Services;
using System.Collections.ObjectModel;
namespace TaskForge.Views;
public partial class ProjectsPage : ContentPage
{
    private readonly ITaskForgeDataService _data; private readonly IAppSession _session; public ObservableCollection<Project> Items { get; } = new();
    public ProjectsPage(ITaskForgeDataService data, IAppSession session) { InitializeComponent(); _data = data; _session = session; BindingContext = this; }
    protected override async void OnAppearing() { base.OnAppearing(); Items.Clear(); foreach (var item in await _data.GetProjectsAsync(_session.CurrentUser?.UserId ?? 0)) Items.Add(item); }
}