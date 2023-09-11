using AISL;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace AutoInstaller.ViewModels;

public sealed partial class InstallViewModel : ObservableObject
{
    public ObservableCollection<string> Programs { get; }
    public ObservableCollection<string> Versions { get; } = new();
    public ObservableCollection<ParameterDataViewModel> Parameters { get; } = new();

    [ObservableProperty] private string _selectedProgram;
    [ObservableProperty] private string _selectedVersion;

    public InstallViewModel()
    {
        Programs = new(ProgramService.FindPrograms());
    }

    [RelayCommand]
    public async void InstallProgram() // check if program is selected
    {
        ProgramData programData = ProgramService.GetProgramData(SelectedProgram, SelectedVersion);
        programData.ParameterList.Clear();
        foreach (var parameter in Parameters)
        {
            programData.ParameterList.Add(parameter.ParameterData);
        }
        string? installedProgramName = ProgramService.GetInstalledProgramNameFromInstaller(programData.InstallerPath);

        if (installedProgramName is not null)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Caption", "A version of this program was detected on your device. Continuing the installation" +
                " will lead to its deletion. Do you wish to continue?",
              ButtonEnum.YesNo);

            var result = await box.ShowAsync();

            if (result == ButtonResult.Yes)
            {
                PowershellExecutor.RunPowershellUninstaller(installedProgramName);
                PowershellExecutor.RunPowershellInstaller(programData);
            }
        }
        else
        {
            PowershellExecutor.RunPowershellInstaller(programData);
        }
    }

    partial void OnSelectedProgramChanged(string value)
    {
        Versions.Clear();
        if (value != null)
        {
            var versions = ProgramService.FindVersionsOfProgram(value);
            versions.ForEach(version => Versions.Add(version));
        }
    }

    partial void OnSelectedVersionChanged(string value)
    {
        Parameters.Clear();
        if (value != null)
        {
            var programData = ProgramService.GetProgramData(SelectedProgram, value);
            if (programData == null) // programData is null if the selected version doesn't have an AISL file associated
            {
                return;
            }
            programData.ParameterList.ForEach(parameter => Parameters.Add(new(parameter)));
        }
    }
}
