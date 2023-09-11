using AISL;
using AutoInstaller.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Microsoft.Win32;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoInstaller.ViewModels;

public sealed partial class InstallViewModel : ObservableObject
{
    private readonly Window _window;
    private readonly NotificationService _notificationService;
    public ObservableCollection<string> Programs { get; }
    public ObservableCollection<string> Versions { get; } = new();
    public ObservableCollection<ParameterDataViewModel> Parameters { get; } = new();

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(InstallProgramCommand)),
        NotifyCanExecuteChangedFor(nameof(SaveParametersCommand)),
        NotifyCanExecuteChangedFor(nameof(LoadParametersCommand))]
    private string _selectedProgram;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(InstallProgramCommand)),
        NotifyCanExecuteChangedFor(nameof(SaveParametersCommand)),
        NotifyCanExecuteChangedFor(nameof(LoadParametersCommand))]
    private string _selectedVersion;

    [ObservableProperty] private bool _copyInstaller;

    [ObservableProperty] private bool _programSectionIsVisibile;

    partial void OnSelectedVersionChanged(string value)
    {
        if (value != null)
        {
            ProgramSectionIsVisibile = true;
        }
        else
        {
            ProgramSectionIsVisibile = false;
        }
    }

    public InstallViewModel(Window window, NotificationService notificationService)
    {
        _window = window;
        _notificationService = notificationService;
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
        return !string.IsNullOrWhiteSpace(SelectedProgram) && !string.IsNullOrWhiteSpace(SelectedVersion);
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
    [RelayCommand(CanExecute = nameof(IsProgramSelected))]
    public async Task SaveParameters()
    {
        List<ParameterData> parameters = new();
        foreach (var parameter in Parameters)
        {
            parameters.Add(parameter.ParameterData);
        }
        string json = ProgramService.SerializeParameters(parameters);

        var topLevel = TopLevel.GetTopLevel(_window);

        // Start async operation to open the dialog.
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save JSON File",
            DefaultExtension = "json",
        });

        if (file is not null)
        {
            // Open writing stream from the file.
            await using var stream = await file.OpenWriteAsync();
            using var streamWriter = new StreamWriter(stream);
            // Write some content to the file.
            await streamWriter.WriteLineAsync(json);
            _notificationService.NotificationText = "The parameters were saved";
        }
    }

    [RelayCommand(CanExecute = nameof(IsProgramSelected))]
    public async Task LoadParameters()
    {
        var topLevel = TopLevel.GetTopLevel(_window);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType> { new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } } }
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);

            var fileContent = await streamReader.ReadToEndAsync();
            try
            {
                List<ParameterData> loadedParameters = ProgramService.DeserializeParameters(fileContent);
                List<ParameterData> setParameters = Parameters.Select(param => param.ParameterData).ToList();

                if (!ProgramService.AreParametersTheSame(setParameters, loadedParameters))
                {
                    throw new Exception();
                }
                Parameters.Clear();
                foreach (var parameter in loadedParameters)
                {
                    Parameters.Add(new ParameterDataViewModel(parameter));
                }
            }
            catch (Exception ex)
            {
                _notificationService.NotificationText = "Incorrect file selected";
            }
        }
    }
}