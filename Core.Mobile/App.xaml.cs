using Autofac.Core;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core
{
    public partial class App : Application
    {
        public static App Instance => (App)Current;

        public readonly IoCContainer Container = new IoCContainer();

        public App(params IModule[] module)
        {
            InitializeComponent();

            RegisterTypes(module);
            ConfigureMap();

            Container.Build();

            //RegisterAppCenter();

            //CryptConfig();
        }

        protected override async void OnStart()
        {
            base.OnStart();            
            await InitializeAsyc();
        }

        void RegisterTypes(params IModule[] module)
        {
            Container.RegisterSingleton<INavigationService, NavigationService>();
            Container.RegisterSingleton<IDialogService, DialogService>();
            Container.RegisterSingleton<IHttpApiRequest, HttpApiRequest>();
            Container.RegisterSingleton<IValidationService, ValidationService>();

            Container.RegisterModules(module);
        }

        void ConfigureMap()
        {
            NavigationService.ConfigureMap<MainViewModel, MainPage>();
        }

        async Task InitializeAsyc()
        {
            var navigationService = Container.Resolve<INavigationService>();

            
          
                await navigationService.NavigateAndClearBackStackAsync<MainViewModel>();
        }

        //void RegisterAppCenter()
        //{
        //    AppCenter.Start(AppConfiguration.AppCenterKey,
        //                    typeof(Crashes));

        //    AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        //    {
        //        var ex = (System.Exception)args.ExceptionObject;
        //        var exception = new ExceptionExtensions(ConcactException(ex));
        //        Crashes.TrackError(exception);
        //    };

        //    Crashes.ShouldProcessErrorReport = (ErrorReport report) =>
        //    {
        //        //return report.Exception is ExceptionExtensions;
        //        return true;
        //    };
        //}

        string ConcactException(Exception ex, StringBuilder str = null)
        {
            if (str == null)
                str = new StringBuilder();

            str.AppendLine($"Message: {ex.Message}");
            str.AppendLine($"StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
                str.AppendLine(ConcactException(ex.InnerException, str));

            return str.ToString();
        }


    }
}
