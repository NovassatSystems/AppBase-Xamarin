using System.ComponentModel;
using Android.Content;
using Core;
using Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                SetControlBackground();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(CustomEntry.HasError))            
                SetControlBackground();            
        }

        void SetControlBackground()
        {
            if ((Element as CustomEntry).HasError)
                Control.Background = Context.GetDrawable(Resource.Drawable.CustomEntryError);
            else if (Element.StyleId == "right54")
                Control.Background = Context.GetDrawable(Resource.Drawable.CustomEntryRight54);
            else
                Control.Background = Context.GetDrawable(Resource.Drawable.CustomEntry);
        }
    }
}