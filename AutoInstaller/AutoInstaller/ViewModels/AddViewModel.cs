using AISL;
using Avalonia.Controls;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace AutoInstaller.ViewModels;

public partial class AddViewModel : ObservableValidator
{
    public List<ParameterType> ParameterTypes { get; } = Enum.GetValues<ParameterType>().ToList();

    //installer settings
    [ObservableProperty,
     NotifyPropertyChangedFor(nameof(HasName), nameof(AreInstallerDetailsSet)),
     NotifyCanExecuteChangedFor(nameof(AddProgramCommand))]
    private string? _name;

    //todo add custom validator class
    [ObservableProperty,
     NotifyPropertyChangedFor(nameof(HasValidDirectory), nameof(AreInstallerDetailsSet)),
     NotifyCanExecuteChangedFor(nameof(AddProgramCommand))]
    private string? _installationsPath;

    [ObservableProperty] private string? _selectedVersion;

    public bool HasName => !string.IsNullOrEmpty(Name);

    // todo: simplify with De Morgan Theorem
    public bool HasValidDirectory => ProgramService.CheckDirectoryValidity(InstallationsPath);

    public bool AreInstallerDetailsSet => HasName && HasValidDirectory;

    // parameter settings
    [ObservableProperty] private ParameterData? _selectedParameter;
    [ObservableProperty] private ParameterType? _selectedParameterType = ParameterType.@string;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasParameterName))]
    [NotifyCanExecuteChangedFor(nameof(AddParameterCommand))]
    private string? _parameterName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasParameterValue))]
    [NotifyCanExecuteChangedFor(nameof(AddParameterCommand))]
    private string? _parameterValue;

    [ObservableProperty] private bool _parameterIsOptional;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddParameterCommand))]
    private bool _parameterIsReadOnly;

    public bool HasParameterName => !string.IsNullOrEmpty(ParameterName);

    public bool HasParameterValue => !string.IsNullOrEmpty(ParameterValue);

    public ObservableCollection<string> Versions { get; set; } = new();
    public ObservableCollection<ParameterData> Parameters { get; set; } = new();

    partial void OnNameChanged(string? value)
    {
        if (string.IsNullOrEmpty(value))
            throw new DataValidationException("Name cannot be empty");
    }



    partial void OnInstallationsPathChanged(string? value)
    {
        Versions.Clear();
        if (string.IsNullOrEmpty(value))
            throw new DataValidationException("Directory cannot be empty");
        if (!Directory.Exists(value))
            throw new DataValidationException("Directory does not exist");
        if (ProgramService.CheckDirectoryValidity(value))
        {
            ProgramService.FindVersionSubdirectories(value!).ForEach(version => Versions.Add(version));
            SelectedVersion = Versions[0];
        }

    }

    private readonly Window _window;
    public AddViewModel(Window window)
    {
        _window = window;
    }

    // todo: simplify logic by having a single Value property and a bool that checks if the user wants a ReadOnly parameter Value (i.e. FixedValue) or ReadWrite (i.e. DefaultValue)
    [RelayCommand(CanExecute = nameof(IsParameterDataValid))]
    private void AddParameter()
    {
        ParameterData parameter = new()
        {
            IsOptional = ParameterIsOptional,
            Type = SelectedParameterType!.Value, // make sure you can't add parameter without selecting a type
            Name = ParameterName!,
            IsReadOnly = ParameterIsReadOnly,
            Value = ParameterValue
        };

        Parameters.Add(parameter);
    }

    [RelayCommand(CanExecute = nameof(IsParameterSelected))]
    private void RemoveParameter()
    {
        Parameters.Remove(SelectedParameter!);
    }

    [RelayCommand(CanExecute = nameof(CanAddProgram))]
    private void AddProgram()
    {
        ProgramData programData = new() // remember to add data here
        {
            Name = Name,
            InstallationsPath = InstallationsPath,
            ParameterList = Parameters.ToList(),
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
        if (!HasParameterName)
            return false;
        if (ParameterIsReadOnly)
            return HasParameterValue;

        return true;
    }

    private bool CanAddProgram()
    {
        return HasName && HasValidDirectory;
    }

    private bool IsParameterSelected()
    {
        return SelectedParameter != null;
    }
}
