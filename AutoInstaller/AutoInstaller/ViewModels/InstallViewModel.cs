﻿using AISL;
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
            //ParameterList = new()
            //{
            //    new ParameterData()
            //    {
            //        Name = "APPDIR",
            //        DefaultValue = "C:\\Users\\sziba\\Desktop\\New folder"
            //    }
            //},
            InstallerPath = @"D:\Summer School 2023\asd\asd\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi",
            Uninstall = true
        };
        PowershellExecutor.RunPowershellInstaller(mockData);
    }
}
