using TaskForge.Models;
using TaskForge.Services;
using System.Collections.ObjectModel;
namespace TaskForge.Views;
public partial class NotificationsPage : ContentPage
{
    private readonly ITaskForgeDataService _data; private readonly IAppSession _session; public ObservableCollection<Notification> Items { get; } = new();
    public NotificationsPage(ITaskForgeDataService data, IAppSession session) { InitializeComponent(); _data = data; _session = session; BindingContext = this; }
    protected override async void OnAppearing() { base.OnAppearing(); Items.Clear(); foreach (var item in await _data.GetNotificationsAsync(_session.CurrentUser?.UserId ?? 0)) Items.Add(item); }
    private async void OnNotificationTapped(object sender, EventArgs e)
    {
        if (sender is TapGestureRecognizer { BindingContext: Notification notification } && !notification.IsRead)
        {
            await _data.MarkNotificationReadAsync(notification.NotificationId);
            notification.IsRead = true;
            OnPropertyChanged(nameof(Items));
        }
    }
}