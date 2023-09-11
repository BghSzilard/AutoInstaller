using AISL;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Core;

public static class ProgramService
{
	private static readonly string _databasePath;
	public static readonly string _programsPath;
	private static readonly string _databaseFolderName = "Database";
	private static readonly string _programsFolderName = "Programs";

	public static string DatabasePath => _databasePath;
	public static string ProgramsPath => _programsPath;

	static ProgramService()
	{
		string databasePath = Enumerable.Range(0, 4).Aggregate(Environment.CurrentDirectory,
			(current, _) => Path.GetDirectoryName(current)!);

		_databasePath = Path.Combine(databasePath, _databaseFolderName);
		if (!Directory.Exists(_databasePath))
		{
			Directory.CreateDirectory(_databasePath);
		}

		_programsPath = Path.Combine(_databasePath, _programsFolderName);
		if (!Directory.Exists(_programsPath))
		{
			Directory.CreateDirectory(_programsPath);
		}
	}

	public static void SaveProgram(ProgramData programData)
	{
		string programPath = Path.Combine(_programsPath, programData.Name!);

		if (!Directory.Exists(programPath))
		{
			Directory.CreateDirectory(programPath);
		}

		string filePath = Path.Combine(programPath, $"config.aisl");
		using StreamWriter writer = new(filePath);

		writer.Write(AISLScriptBuilder.Build(programData));
	}

	public static string FindMostRecentFileInDirectory(string directoryPath)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
		FileInfo[] files = directoryInfo.GetFiles();

		if (files.Length > 0)
		{
			FileInfo mostRecentFile = files.OrderByDescending(f => f.LastWriteTime).First();
			return mostRecentFile.Name;
		}
		else
		{
			throw new Exception("There are no files in this directory");
		}
	}

	// todo: Application shouldn't crash when selecting a valid directory but to which access is denied (right now throws an UnauthorizedException when trying to pass the root drive path)
	//public static List<string> FindVersionSubdirectories(string directoryPath)
	//{
	//    List<string> versionDirectories = FindSubdirectories(directoryPath);
	//    versionDirectories.RemoveAll(directory => Directory.GetFiles(directory, "*.msi").Length == 0);

	//    List<string> versions = new();
	//    versionDirectories.ForEach(directory => versions.Add(directory.Replace(@$"{directoryPath}\", string.Empty)));

	//    return versions;
	//}

	public static List<string> FindPrograms()
	{
		var subdirectories = FindSubdirectories(_programsPath);
		return subdirectories;
	}
	public static List<string> FindSubdirectories(string directoryPath)
	{
		List<string> subdirectories = Directory.GetDirectories(directoryPath).Select(d => new DirectoryInfo(d).Name).ToList();
		return subdirectories;
	}

	public static List<string> FindVersionsOfProgram(ProgramData programData)
	{
		string programPath = programData.InstallationsPath;

		return GetVersions(programPath);
	}

	public static ProgramData GetProgramData(string programName)
	{
		string scriptPath = Path.Combine(_programsPath, programName, $"config.aisl");
		if (File.Exists(scriptPath)) // for versions that don't have AISL files associated
		{
			return ScriptDataExtractor.GetProgramData(scriptPath);
		}

		return null!;
	}

	/// <summary>
	/// This function tests if a Directory is 'Valid' or 'Invalid'.
	/// Of course, validity is relative, but by our definition a 'Valid' Directory is the following:
	/// <list type="bullet">
	/// <item>non-empty string</item>
	/// <item>points to an existing directory in the file system</item>
	/// <item>has at least one subdirectory</item>
	/// </list>
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static bool CheckFilePathValidity(string? path, string? installfolderPath)
	{
		return !string.IsNullOrEmpty(path) && File.Exists(path) && path.Contains(installfolderPath);
	}

	public static List<string> GetVersions(string installationPath)
	{
		return FindSubdirectories(installationPath);
	}

    public static bool CheckFolderPathValidity(string? installationPathString)
    {
	    return !string.IsNullOrEmpty(installationPathString)
	           && Directory.Exists(installationPathString);
    }
    public static void CopyProgramVersion(ProgramData programData, string sourcePath, string version)
    {
        string destinationPath = Path.Combine(_programsPath, programData.Name);
        int lastIndex = sourcePath.LastIndexOf('/');

        string copiedFolderName = sourcePath.Substring(lastIndex + 1);
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }
        string copiedFolderPath = Path.Combine(destinationPath, copiedFolderName);
        programData.InstallationsPath = copiedFolderPath;

        if(!Directory.Exists(copiedFolderPath))
        {
            Directory.CreateDirectory(copiedFolderPath);
        }

        string copiedVersionPath = Path.Combine(copiedFolderPath, version);
        sourcePath = Path.Combine(sourcePath, version);
        if (!Directory.Exists(copiedVersionPath))
        {
            FileSystem.CopyDirectory(sourcePath, copiedVersionPath, UIOption.AllDialogs);
        }
    }
	public static bool IsPathAbsolute(string path)
	{
		return char.IsLetter(path[0]);
	}
    public static string GetProductCode(string selectedProgram)
	{
		string installsPath = Enumerable.Range(0, 4).Aggregate(Environment.CurrentDirectory,
			(current, _) => Path.GetDirectoryName(current)!);

		installsPath = Path.Combine(installsPath, "Installs", selectedProgram);
		string installLogPath = Path.Combine(installsPath, "installLog.txt");
		if (!File.Exists(installLogPath))
		{
			return string.Empty;
		}

		string fileContent = File.ReadAllText(installLogPath);

		// Use regular expressions to find the property value
		string pattern = $@"Property\(S\): {Regex.Escape("ProductCode")} = (.+)";
		Match match = Regex.Match(fileContent, pattern);

		if (match.Success)
		{
			string propertyValue = Regex.Unescape(match.Groups[1].Value);
			return propertyValue.Replace("\r", "");
		}

		return string.Empty;
	}
	public static string SerializeParameters(List<ParameterData> parameters)
	{
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        string json = JsonSerializer.Serialize(parameters, options);
		return json;
	}
	public static List<ParameterData>? DeserializeParameters(string json)
	{
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        List<ParameterData>? parameters = JsonSerializer.Deserialize<List<ParameterData>>(json, options);
		return parameters;
	}
	public static bool AreParametersTheSame(List<ParameterData> setParameters,  List<ParameterData> loadedParameters)
	{
		if (setParameters.Count != loadedParameters.Count)
		{
			return false;
		}
		var setParameterNames = setParameters.Select(p => p.Name).ToList();
		var loadedParameterNames = loadedParameters.Select(p => p.Name).ToList();
		foreach (var parameterName in setParameterNames)
		{
			if (!loadedParameterNames.Contains(parameterName))
			{
				return false;
			}
		}
		return true;
	}
}