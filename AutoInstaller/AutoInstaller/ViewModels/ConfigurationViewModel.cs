using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoInstaller.ViewModels
{
    public sealed partial class ConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _folderPath = @"C:\";
    }
}