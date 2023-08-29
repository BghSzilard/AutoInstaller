using AISL;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

    [ObservableProperty] private ParameterType _selectedParameterType;
    [ObservableProperty] private string? _parameterName;
    [ObservableProperty] private string? _parameterDefaultValue;
    [ObservableProperty] private bool _parameterIsOptional;

    partial void OnInstallationsPathChanged(string? value)
    {
        ProgramService.FindVersionSubdirectories(value!).ForEach(version => Versions.Add(version));
    }

    [RelayCommand(CanExecute = nameof(IsParameterDataValid))]
    public void AddParameter()
    {
        ParameterData parameter = new()
        {
            IsOptional = ParameterIsOptional,
            Type = SelectedParameterType.ToString(),
            Name = ParameterName!,
            DefaultValue = ParameterDefaultValue
        };

        Parameters.Add(parameter);
    }

    [RelayCommand(CanExecute = nameof(IsParameterSelected))]
    public void RemoveParameter()
    {
        Parameters.Remove(SelectedParameter!.Value);
    }

    [RelayCommand(CanExecute = nameof(IsProgramDataValid))]
    public void AddProgram()
    {
        ProgramData programData = new()
        {
            Name = Name,
            InstallationsPath = InstallationsPath,
            ParameterList = Parameters.ToList(),
            Version = SelectedVersion // remember to add data here
        };
        ProgramService.SaveProgram(programData);
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
