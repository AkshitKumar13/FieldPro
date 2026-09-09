using ContosoDashboard.Models;
using ContosoDashboard.Services;
using System.Collections.ObjectModel;
namespace ContosoDashboard.Views;
public partial class DocumentsPage : ContentPage
{
    private readonly IContosoDataService _data; private readonly IAppSession _session; public ObservableCollection<ContosoDocument> Items { get; } = new();
    public DocumentsPage(IContosoDataService data, IAppSession session) { InitializeComponent(); _data = data; _session = session; BindingContext = this; }
    protected override async void OnAppearing() { base.OnAppearing(); Items.Clear(); foreach (var item in await _data.GetDocumentsAsync(_session.CurrentUser?.UserId ?? 0)) Items.Add(item); }
}