using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Core
{
    public abstract class BaseListViewModel<T> : BaseDataViewModel<IEnumerable<T>> where T : class
    {
        public Command<T> ItemClickCommand { get; private set; }
        public ObservableCollection<T> Items { get; private set; }

        protected BaseListViewModel(INavigationService navigationService, string title) : base(navigationService, title)
            => Init();

        protected BaseListViewModel(INavigationService navigationService) : base(navigationService)
            => Init();

        void Init()
        {
            Items = new ObservableCollection<T>();
            ItemClickCommand = new Command<T>(async (item) => await ExecuteItemClickCommand(item));
        }

        protected override async Task SetDataLoadedAsync(IEnumerable<T> data)
            => await Task.Run(() =>
            {
                Items.Clear();
                data?.ForEach((item) => Items.Add(item));
            });

        protected virtual async Task ExecuteItemClickCommand(T item)
            => await Task.FromResult(true);
    }
}
