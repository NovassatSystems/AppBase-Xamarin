using System.Threading.Tasks;

namespace Core
{
    public abstract class BaseDataViewModel<T> : BaseViewModel where T : class
    {
        bool _dataLoaded;
        public bool DataLoaded
        {
            get { return _dataLoaded; }
            private set { SetProperty(ref _dataLoaded, value); }
        }
        protected BaseDataViewModel(INavigationService navigationService, string title) : base(navigationService, title)
            => Init();

        protected BaseDataViewModel(INavigationService navigationService) : base(navigationService)
            => Init();

        void Init()
        {
            DataLoaded = false;
            IsBusy = true;
        }
        public override async Task InitializeAsync(object navigationData)
            => await InitializeAsync();

        protected override async Task InitializeAsync()
            => await LoadDataAsync();

        protected abstract Task<T> GetDataAsync();
        protected abstract Task SetDataLoadedAsync(T data);
        protected virtual async Task OnDataLoadedAsync()
            => await Task.FromResult(DataLoaded = true);
        protected virtual async Task OnDataLoadErrorAsync(ApiResult<T> result)
            => await Task.FromResult(true);
        public async Task RefreshDataAsync()
            => await LoadDataAsync();
        async Task LoadDataAsync()
        {
            IsBusy = true;
            DataLoaded = false;
            var result = await GetDataAsync().Handle();
            if (result?.Data == null || result?.Success == false)
            {
                IsBusy = false;
                await OnDataLoadErrorAsync(result);
                return;
            }
            await SetDataLoadedAsync(result.Data);
            await OnDataLoadedAsync();
            IsBusy = false;
        }
    }
}
