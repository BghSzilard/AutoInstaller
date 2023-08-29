using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AutoInstaller.ViewModels;

public sealed partial class AddViewModel : ObservableObject
{
    private ProgramService _service = new ProgramService();
    [ObservableProperty]
    private string _installationPath;

    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _version;
    public ObservableCollection<string> versions { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<Parameter> Parameters { get; set; } = new ObservableCollection<Parameter>();
    [ObservableProperty]
    private Parameter _selectedParameter = new Parameter();
    [ObservableProperty]
    private Parameter _newParameter = new Parameter();
    public AddViewModel()
    {

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
    public void AddProgram()
    {
        ProgramData programData = new ProgramData();
        programData.Name = Name;
        programData.Version = Version;
        programData.InstallationsPath = InstallationPath;
        //programData.Parameters = Parameters.ToList();
        
        _service.SaveProgram(programData);
    }
    //This function should be in Core
    [RelayCommand]
    public void FindVersions()
    {
        string[] versionDirectories = Directory.GetDirectories(InstallationPath);
        versions.Clear();
        foreach (string versionDirectory in versionDirectories)
        {
            versions.Add(versionDirectory.Replace(InstallationPath + @"\", ""));
        }
    }
}
