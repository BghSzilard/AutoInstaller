﻿using AISL;
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
using Avalonia.Platform.Storage;
using System.Reactive.Joins;

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
	 NotifyPropertyChangedFor(nameof(HasValidFilePath), nameof(AreInstallerDetailsSet)),
	 NotifyCanExecuteChangedFor(nameof(AddProgramCommand))]
	private string? _executablePathString;

	[ObservableProperty] 
	[NotifyPropertyChangedFor(nameof(HasValidFolderPath), nameof(AreInstallerDetailsSet))]
	[NotifyCanExecuteChangedFor(nameof(SelectExecutableCommand), nameof(AddProgramCommand))]
	private string? _installationsPathString;

	[ObservableProperty]
	private IStorageFolder? _installationsFolder;

	[ObservableProperty] 
	private IStorageFile? _executablePath;

	public bool HasName => !string.IsNullOrEmpty(Name);

	public bool HasValidFilePath => ProgramService.CheckFilePathValidity(ExecutablePath?.Path.AbsolutePath.Replace("%20", " "), InstallationsPathString);

	public bool HasValidFolderPath => ProgramService.CheckFolderPathValidity(InstallationsPathString);

	public bool HasInstallationsFolder => !string.IsNullOrEmpty(InstallationsPathString);

	public bool AreInstallerDetailsSet => HasName && HasValidFolderPath && HasValidFilePath;

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
		{
			AddProgramCommand.NotifyCanExecuteChanged();
			throw new DataValidationException("Name cannot be empty");
		}
	}

	partial void OnExecutablePathChanged(IStorageFile? value)
	{
		Versions.Clear();
		if (value == null)
		{
			ExecutablePathString = string.Empty;
			//throw new DataValidationException("Directory cannot be empty");
			return;
		}
		//if (!File.Exists(value.Path.AbsolutePath))
		//	throw new DataValidationException("Directory does not exist");

		string absoluteExecutablePath = value.Path.AbsolutePath;
		if (!absoluteExecutablePath.Contains(InstallationsPathString!))
		{
			//throw new DataValidationException("Executable Path not relative to Installations Path");
			return;
		}
		ExecutablePathString = absoluteExecutablePath.Remove(0, InstallationsPathString!.Length + 1);
		ExecutablePathString = ExecutablePathString.Substring(ExecutablePathString.IndexOf("/") + 1).Replace("%20", " ");
		//ProgramService.GetVersions(_mainApplicationPath).ForEach(version => Versions.Add(version));
		//if (Versions.Count == 0)
		//	throw new DataValidationException("No versions were found");
	}

	//partial void OnInstallationsPathChanged(string? value)
	//{

	//}

	partial void OnInstallationsFolderChanged(IStorageFolder? value)
	{
		if (value != null)
		{
			InstallationsPathString = value.Path.AbsolutePath.Replace("%20", " ");
			ExecutablePath = null;
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
		ParameterData parameter;
		if (ParameterIsReadOnly)
		{
			parameter = new ParameterData
			{
				IsOptional = ParameterIsOptional,
				Type = SelectedParameterType!.Value, // make sure you can't add parameter without selecting a type
				Name = ParameterName!,
				DefaultValue = null,
				FixedValue = ParameterValue
			};
		}
		else
		{
			parameter = new ParameterData
			{
				IsOptional = ParameterIsOptional,
				Type = SelectedParameterType!.Value, // make sure you can't add parameter without selecting a type
				Name = ParameterName!,
				DefaultValue = ParameterValue,
				FixedValue = null
			};
		}

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
		//ProgramData programData = new() // remember to add data here
		//{
		//	Name = Name,
		//	InstallationsPath = InstallationsPath,
		//	ParameterList = Parameters.ToList(),
		//	Version = ApplicationVersion,
		//	Uninstall = true,
		//	InstallerPath = InstallerPath
		//};
		//ProgramService.SaveProgram(programData);
	}

	[RelayCommand]
	private async Task SelectInstallationsFolder()
	{
		var topLevel = TopLevel.GetTopLevel(_window);

		// Start async operation to open the dialog.
		var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
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

		// Start async operation to open the dialog.
		var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
		{
			Title = "Open Installer File",
			AllowMultiple = false,
			FileTypeFilter = new[]{ new FilePickerFileType("Executable")
			{
				Patterns = new[] { "*.exe" }
			},
			new FilePickerFileType("Windows Installer")
			{
				Patterns = new[] { "*.msi" }
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
