using AutoInstaller.Services;
using AutoInstaller.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutoInstaller.ViewModels
{
    public sealed partial class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<string> Programs { get; set; } = new();

        private readonly ServiceCollection _serviceCollection;

        public NotificationService NotificationService { get; }

        [ObservableProperty]
        private UserControl? _content;

        [ObservableProperty]
        private SplitViewDisplayMode _mode = SplitViewDisplayMode.CompactInline;

        [ObservableProperty]
        private bool _isPaneOpen = true;

        public List<PageData> Pages { get; }
        public NavigationService Navigation { get; }

        public MainWindowViewModel(ServiceCollection serviceCollection, PageService pageService, NavigationService navigationService, NotificationService notificationService)
        {
            _serviceCollection = serviceCollection;
            NotificationService = notificationService;
            Navigation = navigationService;
            Pages = pageService.Pages.Select(x => x.Value).ToList();

            if (navigationService.CurrentPageType is not null)
            {
                ButtonClick(pageService.Pages[navigationService.CurrentPageType]);
            }

            navigationService.CurrentPageChanged += type =>
            {
                ButtonClick(pageService.Pages[type]);
            };

            //foreach (var program in ProgramService.FindPrograms())
            //{
            //    Programs.Add(program);
            //}
        }

        [RelayCommand]
        public void ButtonClick(PageData pageData)
        {
            if (Content is { } oldContent)
            {
                DataContextIsActiveChanged(false, oldContent.DataContext);
            }

            var control = _serviceCollection.GetService(pageData.Type!) as UserControl ?? throw new System.Exception("null control");
            control.DataContext = _serviceCollection.GetService(pageData.ViewModelType!);
            Content = control;

            DataContextIsActiveChanged(true, control.DataContext);

            var oldMode = Mode;
            Mode = pageData.ShowSidePanel ? SplitViewDisplayMode.CompactInline : SplitViewDisplayMode.Inline;

            if (pageData.ShowSidePanel is false)
            {
                IsPaneOpen = false;
            }
            else if (IsPaneOpen is false && oldMode is SplitViewDisplayMode.Inline)
            {
                IsPaneOpen = true;
            }
        }

        public static void IsActiveChanged(bool isActive, IActiveAware activeAware)
        {
            if (isActive)
            {
                activeAware.OnActivated();
            }
            else
            {
                activeAware.OnDeactivated();
            }
        }

        private static void DataContextIsActiveChanged(bool isActive, object? dataContext)
        {
            if (dataContext is IActiveAware activeAware)
            {
                IsActiveChanged(isActive, activeAware);
            }
        }

        [RelayCommand]
        public void AddProgram()
        {
            Navigation.CurrentPageType = typeof(AddPage);
        }

        [RelayCommand]
        public void InstallProgram()
        {
            Navigation.CurrentPageType = typeof(InstallPage);
        }
    }
}