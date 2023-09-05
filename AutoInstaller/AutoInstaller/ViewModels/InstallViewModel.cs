using AISL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Collections.ObjectModel;
using System.IO;

namespace AutoInstaller.ViewModels;

public sealed partial class InstallViewModel : ObservableObject
{
    public ObservableCollection<string> Programs { get; }
    public ObservableCollection<string> Versions { get; } = new();
    public ObservableCollection<ParameterDataViewModel> Parameters { get; } = new();

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof (InstallProgramCommand))] private string _selectedProgram;
    [ObservableProperty] private string _selectedVersion;

    public InstallViewModel()
    {
        Programs = new(ProgramService.FindPrograms());
    }

    [RelayCommand(CanExecute = nameof(IsProgramSelected))]
    public async void InstallProgram() // check if program is selected
    {
        ProgramData programData = ProgramService.GetProgramData(SelectedProgram);
        programData.ParameterList.Clear();
        foreach (var parameter in Parameters)
        {
            programData.ParameterList.Add(parameter.ParameterData);
        }
        string? installedProgramName = ProgramService.GetInstalledProgramNameFromInstaller(Path.Combine(programData.InstallationsPath, SelectedVersion, programData.InstallerPath));

        if (installedProgramName is not null)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Caption", "A version of this program was detected on your device. Continuing the installation" +
                " will lead to its deletion. Do you wish to continue?",
              ButtonEnum.YesNo);

            var result = await box.ShowAsync();

            if (result == ButtonResult.Yes)
            {
                //var custommessageBox = new UninstallMessageBox("Please wait while the program is being uninstalled...");
                //custommessageBox.Show();

                var uninstallTask = PowershellExecutor.RunPowershellUninstallerAsync(installedProgramName);

                await uninstallTask;
                //custommessageBox.Close();

                //PowershellExecutor.RunPowershellInstaller(programData, SelectedVersion);
            }
        }
        else
        {
            PowershellExecutor.RunPowershellInstaller(programData, SelectedVersion);
        }
    }
    bool IsProgramSelected()
    {
        return !string.IsNullOrWhiteSpace(SelectedProgram);
    }
    partial void OnSelectedProgramChanged(string value)
    {
        Versions.Clear();
        Parameters.Clear();
        if (value != null)
        {
            var programData = ProgramService.GetProgramData(SelectedProgram);
            var versions = ProgramService.FindVersionsOfProgram(programData);
            versions.ForEach(version => Versions.Add(version));
            programData.ParameterList.ForEach(parameter => Parameters.Add(new(parameter)));
        }
    }

    //partial void OnSelectedVersionChanged(string value)
    //{
    //    if (value != null)
    //    {
    //        var programData = ProgramService.GetProgramData(SelectedProgram, );
    //        if (programData == null)
    //        {
    //            return;
    //        }
    //        programData.ParameterList.ForEach(parameter => Parameters.Add(new(parameter)));
    //    }
    //}
}
