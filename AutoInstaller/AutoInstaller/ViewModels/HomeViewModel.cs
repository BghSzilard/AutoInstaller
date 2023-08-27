using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutoInstaller.ViewModels;

public sealed partial class HomeViewModel : ObservableObject
{
    public ObservableCollection<Parameter> Parameters { get; set; } = new ObservableCollection<Parameter>();
    [ObservableProperty]
    private Parameter _selectedParameter;
    public HomeViewModel() 
    {
        Parameters.Add(new Parameter() { Name = "Appdir", Value = "C:\\users" });
        Parameters.Add(new Parameter() { Name = "Appdir", Value = "C:\\users" });
    }
    [RelayCommand]
    public void RemoveParameter()
    {
        Parameters.Remove(SelectedParameter);
    }
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}