using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Core
{
    public interface INavigationService
    {
        Task NavigateToAsync<TViewModel>(object parameter = null) where TViewModel : IBaseViewModel;
        Task NavigateBackAsync(object parameter = null);
        Task NavigateAndClearBackStackAsync<TViewModel>(object parameter = null) where TViewModel : IBaseViewModel;
        Task NavigateAndClearBackStackAsync(Type type, object parameter = null);
        Task NavigateToRootAsync<TViewModel>(object parameter = null) where TViewModel : IBaseViewModel;
        Task OpenModalAsync<TViewModel>() where TViewModel : IBaseViewModel;
        Task OpenModalAsync<TViewModel>(object parameter) where TViewModel : IBaseViewModel;
        Task CloseModalAsync(object parameter = null);
    }

    public class NavigationService : INavigationService
    {
        readonly Application CurrentApplication = Application.Current;

        static readonly Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        public static void ConfigureMap<TViewModel, TPage>() where TViewModel : IBaseViewModel
                                                          where TPage : Page
        {
            if (!_mappings.ContainsKey(typeof(TViewModel)))
                _mappings.Add(typeof(TViewModel), typeof(TPage));

            App.Instance.Container.RegisterTypes(typeof(TViewModel));
        }

        public async Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : IBaseViewModel
            => await NavigateToAsync(typeof(TViewModel), parameter, false, false);

        public async Task NavigateAndClearBackStackAsync<TViewModel>(object parameter = null) where TViewModel : IBaseViewModel
            => await NavigateToAsync(typeof(TViewModel), parameter, true, false);

        public async Task NavigateBackAsync(object parameter = null)
            => await PopAsync(false, parameter);

        public async Task NavigateToRootAsync<TViewModel>(object parameter) where TViewModel : IBaseViewModel
           => await NavigateToAsync(typeof(TViewModel), parameter, true, false, true);

        async Task NavigateToAsync(Type type, object parameter, bool cleanBackStack, bool modal, bool rootNav = false)
        {
            var page = CreateAndBindPage(type);

            if (page is MasterDetailPage)
            {
                CurrentApplication.MainPage = page;

                var masterPage = (page as MasterDetailPage).Master;
                await (masterPage.BindingContext as IBaseViewModel).InitializeAsync(parameter);
                return;
            }

            var navigationPage = CurrentApplication.MainPage as NavigationPage;
            var masterDetailPage = CurrentApplication.MainPage as MasterDetailPage;
            var isMasterDetailpage = masterDetailPage != null;

            if (isMasterDetailpage)
                navigationPage = masterDetailPage.Detail as NavigationPage;

            //if (navigationPage?.CurrentPage?.BindingContext?.GetType() == page?.BindingContext?.GetType())
            //{
            //    if (isMasterDetailpage)
            //        masterDetailPage.IsPresented = false;
            //    return;
            //}

            if (rootNav)
            {
                IEnumerable<Page> pages = new List<Page>();
                if (navigationPage?.Navigation?.ModalStack?.Count > 0)
                    pages = pages.Concat(navigationPage?.Navigation?.ModalStack);

                if (navigationPage?.Navigation?.NavigationStack?.Count > 0)
                    pages = pages.Concat(navigationPage?.Navigation?.NavigationStack);

                DestroyPages(pages);

                navigationPage = null;
            }

            if (navigationPage == null)
                CurrentApplication.MainPage = new NavigationPage(page);
            else
            {
                if (modal)
                    await navigationPage.Navigation.PushModalAsync(page);
                else
                    await navigationPage.PushAsync(page);
            }

            if (navigationPage != null && cleanBackStack && (navigationPage.Navigation.NavigationStack.Count > 0 ||
                                                             navigationPage.Navigation.ModalStack.Count > 0))
            {
                var existingPages = navigationPage.Navigation.NavigationStack
                                    .Concat(navigationPage.Navigation.ModalStack)
                                    .Where(p => p != page)
                                    .ToList();

                DestroyPages(existingPages);
                existingPages.ForEach((item) => navigationPage.Navigation.RemovePage(item));
            }

            if (isMasterDetailpage)
                masterDetailPage.IsPresented = false;

            await Task.Delay(250);

            await (page.BindingContext as IBaseViewModel).InitializeAsync(parameter);
        }

        public async Task NavigateAndClearBackStackAsync(Type type, object parameter = null)
                => await NavigateToAsync(type, parameter, true, false);

        Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!_mappings.ContainsKey(viewModelType))
                throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");

            return _mappings[viewModelType];
        }

        Page CreateAndBindPage(Type viewModelType)
        {
            var pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
                throw new Exception($"Mapping type for {viewModelType} is not a page");

            var page = Activator.CreateInstance(pageType) as Page;
            var viewModel = App.Instance.Container.Resolve(viewModelType) as IBaseViewModel;
            page.BindingContext = viewModel;
            return page;
        }

        public async Task OpenModalAsync<TViewModel>() where TViewModel : IBaseViewModel
            => await NavigateToAsync(typeof(TViewModel), null, false, true);

        public async Task OpenModalAsync<TViewModel>(object parameter) where TViewModel : IBaseViewModel
             => await NavigateToAsync(typeof(TViewModel), parameter, false, true);

        public async Task CloseModalAsync(object parameter = null)
            => await PopAsync(true, parameter);

        async Task PopAsync(bool modal, object parameter = null)
        {
            var navigationPage = CurrentApplication.MainPage as NavigationPage;
            var masterDetailPage = CurrentApplication.MainPage as MasterDetailPage;

            if (masterDetailPage != null)
                navigationPage = masterDetailPage.Detail as NavigationPage;

            var currentPage = GetCurrentPage(CurrentApplication.MainPage);

            DestroyPage(currentPage);

            if (modal)
                await navigationPage.Navigation.PopModalAsync();
            else
                await navigationPage.PopAsync();

            if (GetCurrentPage(CurrentApplication.MainPage).BindingContext is IBaseViewModel viewModel)
                await viewModel.NavigationBackAsync(parameter);
        }

        Page GetOnNavigatedToTargetFromChild(Page target)
        {
            Page child = null;

            if (target is MasterDetailPage)
                child = ((MasterDetailPage)target).Detail;
            else if (target is TabbedPage)
                child = ((TabbedPage)target).CurrentPage;
            else if (target is CarouselPage)
                child = ((CarouselPage)target).CurrentPage;
            else if (target is NavigationPage)
                child = target.Navigation.NavigationStack.Last();

            if (child != null)
                target = GetOnNavigatedToTargetFromChild(child);

            return target;
        }

        Page GetCurrentPage(Page mainPage)
        {
            var page = mainPage;

            var lastModal = page.Navigation.ModalStack.LastOrDefault();
            if (lastModal != null)
                page = lastModal;

            return GetOnNavigatedToTargetFromChild(page);
        }

        void DestroyPages(IEnumerable<Page> pages)
            => pages.ForEach((item) => DestroyPage(item));

        void DestroyPage(Page page)
        {
            try
            {
                DestroyChildren(page);

                (page.BindingContext as IBaseViewModel).Dispose();
                page.Behaviors?.Clear();
                page.BindingContext = null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot destroy {page}.", ex);
            }
        }

        void DestroyChildren(Page page)
        {
            switch (page)
            {
                case MasterDetailPage mdp:
                    DestroyPage(mdp.Master);
                    DestroyPage(mdp.Detail);
                    break;
                case TabbedPage tabbedPage:
                    foreach (var item in tabbedPage.Children.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
                case CarouselPage carouselPage:
                    foreach (var item in carouselPage.Children.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
                case NavigationPage navigationPage:
                    foreach (var item in navigationPage.Navigation.NavigationStack.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
            }
        }
    }

    public class IoCContainer
    {
        IContainer _container;
        ContainerBuilder _containerBuilder;


        public IoCContainer()
            => _containerBuilder = new ContainerBuilder();

        public T Resolve<T>()
            => _container.Resolve<T>();

        public object Resolve(Type type)
            => _container.Resolve(type);

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
            => _containerBuilder.RegisterType<TImplementation>().As<TInterface>();

        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface
           => _containerBuilder.RegisterType<TImplementation>().As<TInterface>().SingleInstance();

        public void RegisterModules(IModule[] platformSpecificModules)
        {
            foreach (var platformSpecificModule in platformSpecificModules)
                _containerBuilder.RegisterModule(platformSpecificModule);
        }

        public void RegisterType<T>() where T : class
            => _containerBuilder.RegisterType<T>();

        public void RegisterTypes(params Type[] type)
            => _containerBuilder.RegisterTypes(type);

        public void Build()
        {
            if (_container == null)
                _container = _containerBuilder.Build();
        }
    }
}
