using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutoInstaller.ViewModels;

public sealed partial class DemoViewModel : ObservableObject
{   
    public DemoViewModel() 
    {
        //Options.Add(new Option() { Property = "APPDIR", PropertyName="ceva"});
        //Options.Add(new Option() { Property = "APPDIR1", PropertyName = "ceva1" });
        //Options.Add(new Option() { Property = "APPDIR2", PropertyName = "ceva2" });
    }   
    public partial class Option: ObservableObject
    {
        //[ObservableProperty]
        //private string property;
        //[ObservableProperty]
        //private string propertyName;

    } 
    private ObservableCollection<Option> _options;
    public ObservableCollection<Option> Options
    {
        get { return _options; }
        set
        {
            if (_options != value)
            {
                _options = value;
                OnPropertyChanged(nameof(Options));
            }
        }
    }
    
}

