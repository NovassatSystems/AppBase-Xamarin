using System;
using Xamarin.Forms;

namespace Core
{
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create(nameof(HasError),
                typeof(bool),
                typeof(CustomEntry));

        public bool HasError
        {
            get => (bool)GetValue(HasErrorProperty);
            set => SetValue(HasErrorProperty, value);
        }
    }
}
