using CommunityToolkit.Mvvm.ComponentModel;
using Core;
using System.Collections.ObjectModel;

namespace AutoInstaller.ViewModels;

public sealed partial class InstallViewModel : ObservableObject
{
    public ObservableCollection<string> Programs { get; set; } = new();
    public InstallViewModel()
    {
        foreach (var program in ProgramService.FindPrograms())
        {
            Programs.Add(program);
        }
    }
}
