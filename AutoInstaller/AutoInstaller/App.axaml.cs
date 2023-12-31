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
            serviceCollection.AddSingleton<NotificationService>();

            serviceCollection.AddScope<AddPage>();
            serviceCollection.AddScope<AddViewModel>();

            serviceCollection.AddScope<InstallPage>();
            serviceCollection.AddScope<InstallViewModel>();

            PageService pageService = serviceCollection.GetService<PageService>();
            pageService.RegisterPage<AddPage, AddViewModel>("Add");
            pageService.RegisterPage<InstallPage, InstallViewModel>("Install");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceCollection.CreateService<MainWindowViewModel>(),
                };

                serviceCollection.AddSingleton(desktop.MainWindow);
            }

            NavigationService navigationService = serviceCollection.GetService<NavigationService>();
            navigationService.CurrentPageType = typeof(InstallPage);

            base.OnFrameworkInitializationCompleted();
        }
    }
}