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

namespace AutoInstaller.ViewModels;

public partial class AddViewModel : ObservableValidator
{
	public List<ParameterType> ParameterTypes { get; } = Enum.GetValues<ParameterType>().ToList();

	//installer settings
	[ObservableProperty,
	 NotifyPropertyChangedFor(nameof(HasName), nameof(AreInstallerDetailsSet)),
	 NotifyCanExecuteChangedFor(nameof(AddProgramCommand)), Required(ErrorMessage = "Program Name cannot be empty")]
	private string? _name;

	//todo add custom validator class
	[ObservableProperty,
	 NotifyPropertyChangedFor(nameof(HasValidDirectory), nameof(AreInstallerDetailsSet)),
	 NotifyCanExecuteChangedFor(nameof(AddProgramCommand)), Required(ErrorMessage = "Directory cannot be empty")]
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
	[Required(ErrorMessage = "Parameter must have a name")]
	private string? _parameterName;

	[ObservableProperty] private string? _parameterValue;
	[ObservableProperty] private bool _parameterIsOptional;
	[ObservableProperty] private bool _parameterIsReadOnly;

	public bool HasParameterName => !string.IsNullOrEmpty(ParameterName);

	public ObservableCollection<string> Versions { get; set; } = new();
	public ObservableCollection<ParameterData> Parameters { get; set; } = new();

	partial void OnInstallationsPathChanged(string? value)
	{
		Versions.Clear();
		//if (string.IsNullOrEmpty(value))
		//	throw new DataValidationException("Directory cannot be empty");
		//if (!Directory.Exists(value))
		//	throw new DataValidationException("Directory does not exist");
		//if (ProgramService.FindVersionSubdirectories(value!).Count == 0)
		//	throw new DataValidationException("Directory contains no Version folders");
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
		//ParameterData parameter;
		//if (ParameterIsReadOnly)
		//{
		//	parameter = new ParameterData
		//	{
		//		IsOptional = ParameterIsOptional,
		//		Type = SelectedParameterType!.Value, // make sure you can't add parameter without selecting a type
		//		Name = ParameterName!,
		//		DefaultValue = null,
		//		FixedValue = ParameterValue
		//	};
		//}
		//else
		//{
		//	parameter = new ParameterData
		//	{
		//		IsOptional = ParameterIsOptional,
		//		Type = SelectedParameterType!.Value, // make sure you can't add parameter without selecting a type
		//		Name = ParameterName!,
		//		DefaultValue = ParameterValue,
		//		FixedValue = null
		//	};
		//}

		ParameterData parameter = new()
		{
			IsOptional = ParameterIsOptional,
			Type = SelectedParameterType!.Value, // make sure you can't add parameter without selecting a type
			Name = ParameterName!,
			DefaultValue = null,
			FixedValue = "five"
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
		return HasParameterName;
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
