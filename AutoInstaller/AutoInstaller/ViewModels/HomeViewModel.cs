using AutoInstaller.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System.Collections.ObjectModel;

namespace AutoInstaller.ViewModels;

public sealed partial class HomeViewModel : ObservableObject
{
    public ObservableCollection<Parameter> Parameters { get; set; } = new ObservableCollection<Parameter>();
    [ObservableProperty]
    private Parameter _selectedParameter = new Parameter();
    [ObservableProperty]
    private Parameter _newParameter = new Parameter();
    private PowershellExecutor _languageConverter { get; set; }
    public HomeViewModel(PowershellExecutor languageConverter)
    {
        _languageConverter = languageConverter;
    }
    [RelayCommand]
    public void AddParameter()
    {
        if (NewParameter.Name is not null && NewParameter.Value is not null)
        {
            Parameters.Add(NewParameter);
        }
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
}