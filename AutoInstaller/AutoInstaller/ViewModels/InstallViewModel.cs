using AISL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [RelayCommand]
    public void InstallProgram()
    {
        ProgramData mockData = new()
        {
            Name = "Test Installer",
            ParameterList = new()
            {
                new ParameterData()
                {
                    Name = "APPDIR",
                    DefaultValue = @"C:\"
                }
            },
            InstallerPath = @"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi",
            Uninstall = true
        };
        PowershellExecutor.RunPowershellInstaller(mockData);
    }
}
