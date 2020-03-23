namespace Core
{
    public interface IDeviceService
    {
        string DeviceID { get; }
        string PushToken { get; }
        string Platform { get; }
        string FriendlyName { get; }
        bool ApnsSandbox { get; }
    }
}