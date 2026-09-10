namespace TaskForge.Services;

public interface IDeviceCapabilitiesService
{
    Task<bool> CapturePhotoAsync(string fileName, CancellationToken cancellationToken = default);

    Task<Location?> GetCurrentLocationAsync(CancellationToken cancellationToken = default);

    bool IsAccelerometerSupported { get; }

    void StartAccelerometer(Action<AccelerometerData> onReading);

    void StopAccelerometer();
}

public readonly record struct AccelerometerData(double X, double Y, double Z);
