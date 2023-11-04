using AISL;
using AutoInstaller.Services;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoInstaller.ViewModels;

public partial class AddViewModel : ObservableValidator
{
    public List<ParameterType> ParameterTypes { get; } = Enum.GetValues<ParameterType>().ToList();

    private string? _name;

    [CustomValidation(typeof(AddViewModel), nameof(ValidateName))]
    [Required(ErrorMessage = "Program name cannot be empty")]
    public string? Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value, true);
            OnPropertyChanged(nameof(AreInstallerDetailsSet));
            AddProgramCommand.NotifyCanExecuteChanged();
        }

    }

    [ObservableProperty,
     NotifyPropertyChangedFor(nameof(HasValidFilePath), nameof(AreInstallerDetailsSet)),
     NotifyCanExecuteChangedFor(nameof(AddProgramCommand))]
    [Required(ErrorMessage = "Executable path cannot be empty")]
    private string? _executablePathString;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasValidFolderPath), nameof(AreInstallerDetailsSet))]
    [NotifyCanExecuteChangedFor(nameof(SelectExecutableCommand), nameof(AddProgramCommand))]
    [Required(ErrorMessage = "Installations path cannot be empty")]
    private string? _installationsPathString;

    [ObservableProperty]
    private IStorageFolder? _installationsFolder;

    [ObservableProperty]
    private IStorageFile? _executablePath;

    public static ValidationResult ValidateName(string? v)
    {
        var programsList = ProgramService.FindSubdirectories(ProgramService.ProgramsPath);
        if (programsList.Contains(v!))
        {
            return new ValidationResult("Configuration already exists");
        }

        string pattern = "^([a-zA-Z0-9][^*/><?\"|:]*)$";
        if (!Regex.IsMatch(v!, pattern))
        {
            return new ValidationResult("Invalid folder name");
        };

        return ValidationResult.Success!;
    }

    public static ValidationResult ValidateParameterName(string? v, ValidationContext context)
    {
        var addVM = context.ObjectInstance as AddViewModel;
        if (string.IsNullOrEmpty(v) && addVM!.AreInstallerDetailsSet)
        {
            return new ValidationResult("Parameter name cannot be empty");
        }
        if (addVM!.Parameters.Any(p => v!.Equals(p.Name)))
        {
            return new ValidationResult("Parameter already exists");
        }

        return ValidationResult.Success!;
    }

    public static ValidationResult ValidateParameterValue(string? v, ValidationContext context)
    {
        if (string.IsNullOrEmpty(v))
        {
            return new ValidationResult("Value cannot be empty");
        }

        var addVM = context.ObjectInstance as AddViewModel;
        if (addVM!.ParameterIsReadOnly && string.IsNullOrEmpty(v))
            return new ValidationResult("Read-Only parameter must have a value");

        switch (addVM.SelectedParameterType)
        {
            case ParameterType.number:
                if (!int.TryParse(v, out var number))
                {
                    return new ValidationResult("Parameter not a numerical value");
                }

                break;
            case ParameterType.flag:
                if (!v!.Equals("0") && !v.Equals("1"))
                {
                    return new ValidationResult("Parameter not a boolean value");
                }

                break;

        }

        return ValidationResult.Success!;
    }

    public bool HasValidName => !string.IsNullOrEmpty(Name)
                                && !ProgramService.FindSubdirectories(ProgramService.ProgramsPath).Contains(Name);

    public bool HasValidFilePath => ProgramService.CheckFilePathValidity(ExecutablePath?.Path.AbsolutePath.Replace("%20", " "), InstallationsPathString);

    public bool HasValidFolderPath => ProgramService.CheckFolderPathValidity(InstallationsPathString);

    public bool HasInstallationsFolder => !string.IsNullOrEmpty(InstallationsPathString);

    public bool AreInstallerDetailsSet => HasValidName && HasValidFolderPath && HasValidFilePath;

    // parameter settings
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveParameterCommand))]
    private ParameterData? _selectedParameter;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ParameterValue))]
    private ParameterType? _selectedParameterType = ParameterType.@string;

    private string? _parameterName;

    [CustomValidation(typeof(AddViewModel), nameof(ValidateParameterName))]
    public string? ParameterName
    {
        get => _parameterName;
        set
        {
            SetProperty(ref _parameterName, value, true);
            OnPropertyChanged(nameof(HasParameterName));
            AddParameterCommand.NotifyCanExecuteChanged();
        }
    }

    private string? _parameterValue;

    [CustomValidation(typeof(AddViewModel), nameof(ValidateParameterValue))]
    public string? ParameterValue
    {
        get => _parameterValue;
        set
        {
            SetProperty(ref _parameterValue, value, true);
            OnPropertyChanged(nameof(HasParameterValue));
            AddParameterCommand.NotifyCanExecuteChanged();
        }
    }

    [ObservableProperty] private bool _parameterIsOptional;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddParameterCommand))]
    [NotifyPropertyChangedFor(nameof(ParameterValue))]
    private bool _parameterIsReadOnly;

    public bool HasParameterName => !string.IsNullOrEmpty(ParameterName);

    public bool HasParameterValue => !string.IsNullOrEmpty(ParameterValue);

    public ObservableCollection<string> Versions { get; set; } = new();
    public ObservableCollection<ParameterData> Parameters { get; set; } = new();
    partial void OnExecutablePathChanged(IStorageFile? value)
    {
        Versions.Clear();
        if (value == null)
        {
            ExecutablePathString = string.Empty;
        }
        else
        {
            string absoluteExecutablePath = value.Path.AbsolutePath.Replace("%20", " ");
            if (!absoluteExecutablePath.Contains(InstallationsPathString!))
            {
                return;
            }
            ExecutablePathString = absoluteExecutablePath.Remove(0, InstallationsPathString!.Length + 1);
            ExecutablePathString = ExecutablePathString.Substring(ExecutablePathString.IndexOf("/") + 1).Replace("%20", " ");
        }
        
    }

    partial void OnInstallationsFolderChanged(IStorageFolder? value)
    {
        if (value != null)
        {
            InstallationsPathString = value.Path.AbsolutePath.Replace("%20", " ");
            ExecutablePath = null;
        }
    }

    private readonly NotificationService _notificationService;
    private readonly Window _window;
    public AddViewModel(Window window, NotificationService notificationService)
    {
        _window = window;
        _notificationService = notificationService;

        ParameterTypes.Remove(ParameterType.choice);
    }

    [RelayCommand(CanExecute = nameof(IsParameterDataValid))]
    private void AddParameter()
    {
        ParameterData parameter = new()
        {
            IsOptional = ParameterIsOptional,
            Type = SelectedParameterType!.Value,
            Name = ParameterName!,
            IsReadOnly = ParameterIsReadOnly,
            Value = ParameterValue
        };

        Parameters.Add(parameter);
        ParameterName = string.Empty;
        ParameterValue = string.Empty;
        ParameterIsReadOnly = false;
        ParameterIsOptional = false;
        SelectedParameterType = ParameterType.@string;
    }

    [RelayCommand(CanExecute = nameof(IsParameterSelected))]
    private void RemoveParameter()
    {
        Parameters.Remove(SelectedParameter!);
    }

    [RelayCommand(CanExecute = nameof(CanAddProgram))]
    private void AddProgram()
    {
        ProgramData programData = new()
        {
            Name = Name?.Trim(' '),
            InstallationsPath = InstallationsPathString,
            ParameterList = Parameters.ToList(),
            InstallerPath = ExecutablePathString
        };
        ProgramService.SaveProgram(programData);

        _notificationService.NotificationText = $"{Name} was added to your list of programs";
    }

    [RelayCommand]
    private async Task SelectInstallationsFolder()
    {
        var topLevel = TopLevel.GetTopLevel(_window);

        var folders = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open Folder",
            AllowMultiple = false
        });

        if (folders.Count >= 1)
        {
            InstallationsFolder = folders[0];
        }
    }

    [RelayCommand(CanExecute = nameof(CanSelectExecutable))]
    private async Task SelectExecutable()
    {
        var topLevel = TopLevel.GetTopLevel(_window);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Installer File",
            AllowMultiple = false,
            FileTypeFilter = new[]{ new FilePickerFileType("Exe / Msi")
            {
                Patterns = new[] { "*.exe", "*.msi" }
            }
            },
            SuggestedStartLocation = InstallationsFolder
        });

        if (files.Count >= 1)
        {
            ExecutablePath = files[0];
        }
    }

    private bool CanSelectExecutable()
    {
        return HasInstallationsFolder;
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
        return AreInstallerDetailsSet;
    }

    private bool IsParameterSelected()
    {
        return SelectedParameter != null;
    }
}