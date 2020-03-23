using Autofac;
using Core;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var navigationBarAppearace = UINavigationBar.Appearance;
            navigationBarAppearace.TintColor = Xamarin.Forms.Color.FromHex("#0A8944").ToUIColor(); // Back buttons and such
            navigationBarAppearace.BarTintColor = Xamarin.Forms.Color.FromHex("#ffffff").ToUIColor();  // Bar's background color
            navigationBarAppearace.SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = Xamarin.Forms.Color.FromHex("#494949").ToUIColor()
            });
            ImageCircleRenderer.Init();

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new Core.App(new PlatformSpecificModule()));

            return base.FinishedLaunching(app, options);
        }       
    }

    public class PlatformSpecificModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DeviceService>().As<IDeviceService>().SingleInstance();
            base.Load(builder);
        }
    }
}
