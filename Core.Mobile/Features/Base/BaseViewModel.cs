using System;
using System.Threading.Tasks;

namespace Core
{
    public interface IBaseViewModel : IDisposable
    {
        Task InitializeAsync(object navigationData);
        Task NavigationBackAsync(object parameter);
    }

    public class BaseViewModel : BindingObject, IBaseViewModel
    {
        protected readonly INavigationService _navigationService;

        private string _title;
        public string Title
        {
            get => _title;
            protected set => SetProperty(ref _title, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        protected BaseViewModel(INavigationService navigationService, string title)
        {
            _navigationService = navigationService;
            Title = title;
        }

        protected BaseViewModel(INavigationService navigationService)
            => _navigationService = navigationService;

        public virtual async Task InitializeAsync(object navigationData)
            => await InitializeAsync();

        protected virtual async Task InitializeAsync()
            => await Task.FromResult(true);

        public virtual async Task NavigationBackAsync(object parameter)
            => await Task.FromResult(true);

        public virtual void Dispose() { }
    }
}
