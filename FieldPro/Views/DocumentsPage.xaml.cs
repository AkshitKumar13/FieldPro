using TaskForge.Models;
using TaskForge.Services;
using System.Collections.ObjectModel;
namespace TaskForge.Views;
public partial class DocumentsPage : ContentPage
{
    private readonly ITaskForgeDataService _data; private readonly IAppSession _session; public ObservableCollection<TaskForgeDocument> Items { get; } = new();
    public DocumentsPage(ITaskForgeDataService data, IAppSession session) { InitializeComponent(); _data = data; _session = session; BindingContext = this; }
    protected override async void OnAppearing() { base.OnAppearing(); Items.Clear(); foreach (var item in await _data.GetDocumentsAsync(_session.CurrentUser?.UserId ?? 0)) Items.Add(item); }

    private async void OnOpenDocumentClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: TaskForgeDocument document } ||
            string.IsNullOrWhiteSpace(document.FilePath))
        {
            return;
        }

        try
        {
            var localPath = Path.Combine(FileSystem.AppDataDirectory, document.FilePath);
            if (!File.Exists(localPath))
            {
                await using var source = await FileSystem.OpenAppPackageFileAsync(document.FilePath);
                await using var destination = File.Create(localPath);
                await source.CopyToAsync(destination);
            }

            var opened = await Launcher.Default.OpenAsync(
                new OpenFileRequest(document.OriginalFileName, new ReadOnlyFile(localPath)));
            if (!opened)
            {
                await DisplayAlertAsync("Document", "No PDF application is available on this device.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Document", $"Unable to open the document. {ex.Message}", "OK");
        }
    }
}