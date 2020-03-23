
using System.ComponentModel;
using Core;
using CoreGraphics;
using iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace iOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        bool _disposed;
        CGColor _defaultBorderColor => Color.FromHex("#1e000000").ToUIColor().CGColor;
        CGColor _focusedBorderColor => Color.FromHex("#007aff").ToUIColor().CGColor;
        CGColor _errorBorderColor => Color.FromHex("#bb060c").ToUIColor().CGColor;     

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.LeftView = new UIKit.UIView(new CGRect(0, 0, 54, 16));
                Control.LeftViewMode = UIKit.UITextFieldViewMode.Always;

                float rightPadding = 16;

                if (Element.StyleId == "right54")
                    rightPadding = 54;

                Control.RightView = new UIKit.UIView(new CGRect(0, 0, rightPadding, 16));
                Control.RightViewMode = UIKit.UITextFieldViewMode.Always;

                Control.Layer.CornerRadius = 5;

                SetControlStyle(false);

                Element.Focused += Element_Focused;
                Element.Unfocused += Element_Focused;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(CustomEntry.HasError))
                SetControlStyle(false);
        }

        private void Element_Focused(object sender, FocusEventArgs e)
            => SetControlStyle(e.IsFocused);

        void SetControlStyle(bool focused)
        {
            if ((Element as CustomEntry).HasError)
            {
                Control.Layer.BorderWidth = 2;
                Control.Layer.BorderColor = _errorBorderColor;
                return;
            }

            if (focused)
            {
                Control.Layer.BorderWidth = 2;
                Control.Layer.BorderColor = _focusedBorderColor;
            }
            else
            {
                Control.Layer.BorderWidth = 1;
                Control.Layer.BorderColor = _defaultBorderColor;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                if (Element != null)
                {
                    Element.Unfocused -= Element_Focused;
                    Element.Focused -= Element_Focused;
                }

                base.Dispose(disposing);
            }
        }
    }
}

