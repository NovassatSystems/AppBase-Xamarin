using Core;
using iOS.Hardware;
using UIKit;
using Xamarin.Forms.Internals;

namespace iOS
{
    public class DeviceService : IDeviceService
    {
        [Preserve]
        public DeviceService()
        {

        }

        public string Platform => "ios";
        public string DeviceID => UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        public string PushToken => string.Empty;//APNSToken;
        public string FriendlyName => DeviceModel.Model(DeviceHardware.HardwareModel);
        public bool ApnsSandbox => false;
    }
}