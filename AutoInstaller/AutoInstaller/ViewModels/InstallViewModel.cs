using AISL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace AutoInstaller.ViewModels;

public sealed partial class InstallViewModel : ObservableObject
{
	public ObservableCollection<string> Programs { get; }
	public ObservableCollection<string> Versions { get; } = new();
	public ObservableCollection<ParameterDataViewModel> Parameters { get; } = new();

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof (InstallProgramCommand))] private string _selectedProgram;
    [ObservableProperty] private string _selectedVersion;
    [ObservableProperty] private bool _copyInstaller;
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

        if (CopyInstaller)
        {
            ProgramService.CopyProgramVersion(programData, programData.InstallationsPath, SelectedVersion);
        }

        
        //string? installedProgramName = ProgramService.GetInstalledProgramNameFromInstaller(Path.Combine(programData.InstallationsPath, SelectedVersion, programData.InstallerPath));

		string? productCode = ProgramService.GetProductCode(SelectedProgram);

		//user might have deleted installLog manually
		if (string.IsNullOrEmpty(productCode))
		{
			PowershellExecutor.RunPowershellInstaller(programData, SelectedVersion, true);
			return;
		}

		bool isInRegistry = false;
		using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
		using (var key = hklm.OpenSubKey(@$"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{productCode}"))
		{
			if (key != null)
				isInRegistry = true;
		}

		if (!isInRegistry)
		{
			PowershellExecutor.RunPowershellInstaller(programData, SelectedVersion, true);
			return;
		}

		var box = MessageBoxManager
			.GetMessageBoxStandard("Caption", "A version of this program was detected on your device. Continuing the installation" +
			" will lead to its deletion. Do you wish to continue?",
		  ButtonEnum.YesNo);

		var result = await box.ShowAsync();

		if (result == ButtonResult.No)
			return;

		string installsPath = Enumerable.Range(0, 4).Aggregate(Environment.CurrentDirectory,
			(current, _) => Path.GetDirectoryName(current)!);

		installsPath = Path.Combine(installsPath, "Installs", SelectedProgram);
		string installLogPath = Path.Combine(installsPath, "installLog.txt");
		if (File.Exists(installLogPath))
		{
			File.Delete(installLogPath);
		}

		await PowershellExecutor.RunPowershellUninstallerAsync(productCode);

		PowershellExecutor.RunPowershellInstaller(programData, SelectedVersion, true);
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
