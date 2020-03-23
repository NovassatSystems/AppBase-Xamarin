using Android.Content;
using Android.Support.V4.App;
using Droid;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(TransitionNavigationPageRenderer))]
namespace Droid
{
    public class TransitionNavigationPageRenderer : NavigationPageRenderer
    {
        [Preserve]
        public TransitionNavigationPageRenderer(Context context) : base(context)
        {
        }

        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            if (isPush)
            {
                transaction.SetCustomAnimations(Resource.Animation.enter_right, Resource.Animation.exit_left,
                                                Resource.Animation.enter_left, Resource.Animation.exit_right);
            }
            else
            {
                transaction.SetCustomAnimations(Resource.Animation.enter_left, Resource.Animation.exit_right,
                                                Resource.Animation.enter_right, Resource.Animation.exit_left);
            }
        }
    }
}
