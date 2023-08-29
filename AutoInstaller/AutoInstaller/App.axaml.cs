using AISL;
using AutoInstaller.Services;
using AutoInstaller.ViewModels;
using AutoInstaller.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Core;

namespace AutoInstaller
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(serviceCollection);
            serviceCollection.AddSingleton<PageService>();
            serviceCollection.AddSingleton<NavigationService>();

            serviceCollection.AddSingleton<HomePage>();
            serviceCollection.AddSingleton<HomeViewModel>();

            serviceCollection.AddSingleton<AddPage>();
            serviceCollection.AddSingleton<AddViewModel>();

            serviceCollection.AddSingleton<InstallPage>();
            serviceCollection.AddSingleton<InstallViewModel>();

            serviceCollection.AddSingleton<ConfigurationPage>();
            serviceCollection.AddSingleton<ConfigurationViewModel>();

            serviceCollection.AddSingleton<PowershellExecutor>();

            serviceCollection.AddSingleton<ScriptInfoExtractor>();

            PageService pageService = serviceCollection.GetService<PageService>();
            pageService.RegisterPage<HomePage, HomeViewModel>("Demo");
            pageService.RegisterPage<AddPage, AddViewModel>("Add");
            pageService.RegisterPage<InstallPage, InstallViewModel>("Install");
            pageService.RegisterPage<ConfigurationPage, ConfigurationViewModel>("Configuration");

            NavigationService navigationService = serviceCollection.GetService<NavigationService>();
            navigationService.CurrentPageType = typeof(HomePage);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceCollection.CreateService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}