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

            serviceCollection.AddSingleton<DemoPage>();
            serviceCollection.AddSingleton<DemoViewModel>();

            PageService pageService = serviceCollection.GetService<PageService>();
            pageService.RegisterPage<DemoPage, DemoViewModel>("Demo");

            NavigationService navigationService = serviceCollection.GetService<NavigationService>();
            navigationService.CurrentPageType = typeof(DemoPage);

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