using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskForge.Models;
using TaskForge.Services;

namespace TaskForge.ViewModels;

public partial class NotificationsViewModel : ObservableObject
{
    private readonly ITaskForgeDataService _data;
    private readonly IAppSession _session;

    public ObservableCollection<Notification> Items { get; } = new();

    public NotificationsViewModel(ITaskForgeDataService data, IAppSession session)
    {
        _data = data;
        _session = session;
    }

    public async Task LoadAsync()
    {
        Items.Clear();
        foreach (var item in await _data.GetNotificationsAsync(_session.CurrentUser?.UserId ?? 0))
            Items.Add(item);
    }

    [RelayCommand]
    private async Task MarkReadAsync(Notification? notification)
    {
        if (notification is null || notification.IsRead) return;
        await _data.MarkNotificationReadAsync(notification.NotificationId);
        notification.IsRead = true;
        OnPropertyChanged(nameof(Items));
    }
}
