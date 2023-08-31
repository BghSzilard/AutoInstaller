using AISL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System.Collections.ObjectModel;

namespace AutoInstaller.ViewModels;

public sealed partial class InstallViewModel : ObservableObject
{
    public ObservableCollection<string> Programs { get; set; } = new();
    public ObservableCollection<string> Versions { get; set; } = new();
    public ObservableCollection<ParameterData> Parameters { get; set; } = new();
    [ObservableProperty] private string _selectedProgram;
    [ObservableProperty] private string _selectedVersion;
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
        //ProgramData mockData = new()
        //{
        //    Name = "Test Installer",
        //    ParameterList = new()
        //    {
        //        new ParameterData()
        //        {
        //            Name = "APPDIR",
        //            DefaultValue = @"C:\"
        //        }
        //    },
        //    InstallerPath = @"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi",
        //    Uninstall = true
        //};
        ProgramData programData = new()
        {
            Name = SelectedProgram,
            ParameterList = new(),
            InstallerPath = ProgramService.GetAISLScriptData(SelectedProgram, SelectedVersion).InstallerPath
        };
        foreach (var param  in Parameters)
        {
            programData.ParameterList.Add(param);
        }
        PowershellExecutor.RunPowershellInstaller(programData);
    }
    partial void OnSelectedProgramChanged(string value)
    {
        Versions.Clear();
        var programInfo = ScriptInfoExtractor.GetProgramInfo(value);
        var path = programInfo.InstallationsPath;
        foreach (var directory in ProgramService.FindVersionSubdirectories(path))
        {
            Versions.Add(directory);
        }
    }
    partial void OnSelectedVersionChanged(string value)
    {
        Parameters.Clear();
        var programInfo = ScriptInfoExtractor.GetVersionInfo(SelectedProgram, value);
        foreach (var param in programInfo.ParameterList)
        {
            Parameters.Add(param);
        }
    }
}
