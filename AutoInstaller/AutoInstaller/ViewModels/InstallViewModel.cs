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
            InstallationsPath = @"C:\Users\sziba\Desktop\BraveBrowserSetup-BRV010.exe",
            ParameterList = new()
            {
                new ParameterData()
                {
                    Type = ParameterType.number,
                    Name = "Port",
                    DefaultValue = "8080",
                },
                new ParameterData()
                {
                    Type = ParameterType.@string,
                    Name = "ServerName",
                },
                new ParameterData()
                {
                    Type = ParameterType.choice,
                    Name = "DropDown",
                    Options = new() { "option1", "option2" },
                },
                new ParameterData()
                {
                    Type = ParameterType.flag,
                    Name = "Tick",
                },
                new ParameterData()
                {
                    Type = ParameterType.@string,
                    Name = "FixedParameter",
                    FixedValue = "FixedValue"
                },
                new ParameterData()
                {
                    IsOptional = true,
                    Type = ParameterType.@string,
                    Name = "OptionalValue"
                }
            },
            InstallerPath = @"C:\Users\sziba\Desktop\BraveBrowserSetup-BRV010.exe",
            Uninstall = true
        };
    }
}
