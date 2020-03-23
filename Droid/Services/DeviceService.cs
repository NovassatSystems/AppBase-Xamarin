using System.Text.RegularExpressions;
using Android.Net.Wifi;
using Android.OS;
using Android.Telephony;
using Core;
using Xamarin.Forms.Internals;

namespace Droid
{
    public class DeviceService : IDeviceService
    {
        [Preserve]
        public DeviceService()
        {

        }

        public string Platform => "google";
        public string DeviceID
        {
            get
            {
                try
                {
                    TelephonyManager tm = (TelephonyManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.TelephonyService);
                    var deviceId = tm.DeviceId;

                    if (string.IsNullOrEmpty(deviceId))
                    {
                        WifiManager wm = (WifiManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.WifiService);

                        Regex macAddressRegex = new Regex(@"[^\p{L}\p{N}]+");
                        var macAddress = macAddressRegex.Replace(wm.ConnectionInfo.MacAddress, "");

                        return string.Concat(deviceId, Android.OS.Build.Id, macAddress);
                    }

                    return deviceId;
                }
                catch (Java.Lang.SecurityException)
                {
                    return string.Concat(Android.OS.Build.Id, Android.OS.Build.Model);
                }
            }
        }
        public string PushToken => string.Empty;//FirebaseInstanceId.Instance.Token;

        public string FriendlyName =>
                string.IsNullOrEmpty(Build.Model) ?
                string.Empty :
                Build.Model.StartsWith(Build.Manufacturer) ?
                Build.Model :
                string.Format("{0} {1}", Build.Manufacturer, Build.Model);
        public bool ApnsSandbox => false;
    }
}