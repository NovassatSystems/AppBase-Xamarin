using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty<TValue>(ref TValue prop, TValue value, [CallerMemberName] string propertyName = "") {
            prop = value;
            RaisePropertyChanged(propertyName);
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
         => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
