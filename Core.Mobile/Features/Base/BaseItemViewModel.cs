using System.Threading.Tasks;

namespace Core
{
    public abstract class BaseItemViewModel<T> : BaseDataViewModel<T> where T : class
    {
        T _item;
        public T Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        protected BaseItemViewModel(INavigationService navigationService, string title) : base(navigationService, title) { }
        protected BaseItemViewModel(INavigationService navigationService) : base(navigationService) { }

        protected override async Task SetDataLoadedAsync(T data)
            => await Task.FromResult(Item = data);
    }
}
