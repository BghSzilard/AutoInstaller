using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutoInstaller.ViewModels;

public sealed partial class DemoViewModel : ObservableObject
{
    public ObservableCollection<Parameter> Parameters { get; set; } = new ObservableCollection<Parameter>();
    public DemoViewModel() 
    {
        Parameters.Add(new Parameter() { Name = "Appdir", Value = "C:\\users" });
    }
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}