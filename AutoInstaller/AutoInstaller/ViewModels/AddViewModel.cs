using AISL;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoInstaller.ViewModels;

public partial class AddViewModel : ObservableObject
{
    public List<ParameterType> ParameterTypes { get; } = Enum.GetValues<ParameterType>().ToList();

    [ObservableProperty] private string? _name;
    [ObservableProperty] private string? _installationsPath;
    [ObservableProperty] private string? _selectedVersion;

    public ObservableCollection<string> Versions { get; set; } = new();
    public ObservableCollection<ParameterData> Parameters { get; set; } = new();

    [ObservableProperty] private ParameterData? _selectedParameter;

    [ObservableProperty] private ParameterType? _selectedParameterType = null;
    [ObservableProperty] private string? _parameterName;
    [ObservableProperty] private string? _parameterDefaultValue;
    [ObservableProperty] private string? _parameterFixedValue;
    [ObservableProperty] private bool _parameterIsOptional;

    [ObservableProperty] private bool _isWithoutValueChecked = true;
    [ObservableProperty] private bool _isWithDefaultValueChecked = false;
    [ObservableProperty] private bool _isWithFixedValueChecked = false;

    partial void OnInstallationsPathChanged(string? value)
    {
        Versions.Clear();
        ProgramService.FindVersionSubdirectories(value!).ForEach(version => Versions.Add(version));
    }

    private readonly Window _window;
    public AddViewModel(Window window)
    {
        _window = window;
    }

    [RelayCommand(CanExecute = nameof(IsParameterDataValid))]
    private void AddParameter()
    {
        ParameterData parameter = new()
        {
            IsOptional = ParameterIsOptional,
            Type = SelectedParameterType!.Value, // make sure you can't add parameter without selecting a type
            Name = ParameterName!,
            DefaultValue = ParameterDefaultValue,
            FixedValue = ParameterFixedValue
        };

        Parameters.Add(parameter);
    }

    [RelayCommand(CanExecute = nameof(IsParameterSelected))]
    private void RemoveParameter()
    {
        Parameters.Remove(SelectedParameter!);
    }

    [RelayCommand(CanExecute = nameof(IsProgramDataValid))]
    private void AddProgram()
    {
        ProgramData programData = new() // remember to add data here
        {
            Name = Name,
            InstallationsPath = InstallationsPath,
            ParameterList = Parameters.ToList(),
            Version = SelectedVersion,
            Uninstall = true, // hardcoded for now, will be changed
            InstallerPath = ProgramService.FindInstallerPath(Path.Combine(InstallationsPath!, SelectedVersion!))
        };
        ProgramService.SaveProgram(programData);
    }

    [RelayCommand]
    private async Task SelectInstallationsFolder()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select a folder",
        };

        var selectedFolder = await dialog.ShowAsync(_window);

        if (!string.IsNullOrEmpty(selectedFolder))
        {
            InstallationsPath = selectedFolder;
        }
    }

    private bool IsParameterDataValid()
    {
        return true;
    }

    private bool IsProgramDataValid()
    {
        return true;
    }

    private bool IsParameterSelected()
    {
        return (SelectedParameter != null);
    }
}
