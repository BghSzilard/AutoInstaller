using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace AutoInstaller.ViewModels;

public sealed partial class HomeViewModel : ObservableObject
{
    public ObservableCollection<Parameter> Parameters { get; set; } = new ObservableCollection<Parameter>();
    [ObservableProperty]
    private Parameter _selectedParameter;
    private PowershellExecutor _languageConverter { get; set; }
    public HomeViewModel(PowershellExecutor languageConverter)
    {
        _languageConverter = languageConverter;
        Parameters.Add(new Parameter() { Name = "Appdir", Value = "C:\\users" });
        Parameters.Add(new Parameter() { Name = "Appdir", Value = "C:\\users" });
    }
    [RelayCommand]
    public void RemoveParameter()
    {
        Parameters.Remove(SelectedParameter);
    }
    [RelayCommand]
    public void InstallProgram()
    {
        _languageConverter.RunPowershellInstaller();
    }
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}