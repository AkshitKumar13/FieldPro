using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace TaskForge.Services;

public sealed class DeviceCapabilitiesService : IDeviceCapabilitiesService
{
    public bool IsAccelerometerSupported => Accelerometer.Default.IsSupported;

    public async Task<bool> CapturePhotoAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var permission = await Permissions.RequestAsync<Permissions.Camera>();
        if (permission != PermissionStatus.Granted || !MediaPicker.Default.IsCaptureSupported)
        {
            return false;
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        if (photo is null)
        {
            return false;
        }

        var destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        await using var source = await photo.OpenReadAsync();
        await using var destination = File.Create(destinationPath);
        await source.CopyToAsync(destination, cancellationToken);
        return true;
    }

    public async Task<Location?> GetCurrentLocationAsync(CancellationToken cancellationToken = default)
    {
        var permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (permission != PermissionStatus.Granted)
        {
            return null;
        }

        return await Geolocation.Default.GetLocationAsync(
            new GeolocationRequest(GeolocationAccuracy.Medium),
            cancellationToken);
    }

    public void StartAccelerometer(Action<AccelerometerData> onReading)
    {
        if (Accelerometer.Default.IsMonitoring)
        {
            return;
        }

        Accelerometer.Default.ReadingChanged += HandleReadingChanged;
        Accelerometer.Default.Start(SensorSpeed.UI);

        void HandleReadingChanged(object? sender, AccelerometerChangedEventArgs args)
        {
            onReading(new AccelerometerData(
                args.Reading.Acceleration.X,
                args.Reading.Acceleration.Y,
                args.Reading.Acceleration.Z));
        }
    }

    public void StopAccelerometer()
    {
        if (!Accelerometer.Default.IsMonitoring)
        {
            return;
        }

        Accelerometer.Default.Stop();
    }
}
