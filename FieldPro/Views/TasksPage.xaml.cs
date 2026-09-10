namespace TaskForge.Views;
[QueryProperty(nameof(StatusFilter), "status")]
public partial class TasksPage : ContentPage
{
    private readonly ViewModels.TasksViewModel _viewModel;

    public string? StatusFilter
    {
        get => _viewModel.StatusFilter;
        set => _viewModel.StatusFilter = value;
    }

    public TasksPage(ViewModels.TasksViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private void OnStatusChanged(object? sender, EventArgs e)
    {
        if (sender is Picker { BindingContext: Models.TaskItem task })
        {
            _viewModel.UpdateStatusCommand.Execute(task);
        }
    }
}