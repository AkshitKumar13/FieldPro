using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskForge.Services;

namespace TaskForge.ViewModels;

public partial class DeviceCapabilitiesViewModel : ObservableObject
{
    private readonly IDeviceCapabilitiesService _device;

    [ObservableProperty]
    private string cameraStatus = "Ready to request camera permission.";

    [ObservableProperty]
    private string locationStatus = "Ready to request location permission.";

    [ObservableProperty]
    private string accelerometerStatus = "Not monitoring.";

    [ObservableProperty]
    private bool isAccelerometerRunning;

    public bool IsAccelerometerSupported => _device.IsAccelerometerSupported;

    public DeviceCapabilitiesViewModel(IDeviceCapabilitiesService device)
    {
        _device = device;
    }

    [RelayCommand]
    private async Task CapturePhotoAsync()
    {
        var fileName = $"TaskForge_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
        var captured = await _device.CapturePhotoAsync(fileName);
        CameraStatus = captured
            ? $"Photo saved: {fileName}"
            : "Camera permission was denied, unavailable, or cancelled.";
    }

    [RelayCommand]
    private async Task GetLocationAsync()
    {
        var location = await _device.GetCurrentLocationAsync();
        LocationStatus = location is null
            ? "Location permission was denied or location is unavailable."
            : $"Latitude: {location.Latitude:F5}, Longitude: {location.Longitude:F5}";
    }

    [RelayCommand]
    private void StartAccelerometer()
    {
        if (!IsAccelerometerSupported)
        {
            AccelerometerStatus = "Accelerometer is not supported on this device.";
            return;
        }

        _device.StartAccelerometer(reading => MainThread.BeginInvokeOnMainThread(() =>
        {
            AccelerometerStatus = $"X: {reading.X:F2}  Y: {reading.Y:F2}  Z: {reading.Z:F2}";
        }));
        IsAccelerometerRunning = true;
    }

    [RelayCommand]
    private void StopAccelerometer()
    {
        _device.StopAccelerometer();
        IsAccelerometerRunning = false;
        AccelerometerStatus = "Not monitoring.";
    }

    public void StopMonitoring()
    {
        if (IsAccelerometerRunning)
        {
            StopAccelerometer();
        }
    }
}
