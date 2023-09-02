﻿using AISL;
using System.IO;

namespace Core;

public static class ProgramService
{
    private static readonly string _databasePath;
    public static readonly string _programsPath;
    private static readonly string _databaseFolderName = "Database";
    private static readonly string _programsFolderName = "Programs";

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

        string filePath = Path.Combine(programPath, $"{programData.Version}.aisl");
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

    public static List<string> FindSubdirectories(string directoryPath)
    {
        List<string> subdirectories = Directory.GetDirectories(directoryPath).ToList();
        return subdirectories;
    }

    // todo: Application shouldn't crash when selecting a valid directory but to which access is denied (right now throws an UnauthorizedException when trying to pass the root drive path)
    public static List<string> FindVersionSubdirectories(string directoryPath)
    {
        List<string> versionDirectories = FindSubdirectories(directoryPath);
        versionDirectories.RemoveAll(directory => Directory.GetFiles(directory, "*.msi").Length == 0);

        List<string> versions = new();
        versionDirectories.ForEach(directory => versions.Add(directory.Replace(@$"{directoryPath}\", string.Empty)));

        return versions;
    }

    public static List<string> FindPrograms()
    {
        var subdirectories = FindSubdirectories(_programsPath);
        foreach (var index in Enumerable.Range(0, subdirectories.Count))
        {
            subdirectories[index] = subdirectories[index]
                .Replace(_programsPath, "")
                .Replace("\\", "");
        }
        return subdirectories;
    }

    public static string FindInstallerPath(string versionPath)
    {
        var installerPath = Directory.GetFiles(versionPath, "*.msi");
        if (installerPath.Length == 0)
        {
            throw new Exception("Couldn't find any files with msi extension");
        }
        if (installerPath.Length > 1)
        {
            throw new Exception("More files with msi extension found");
        }
        return installerPath[0];
    }

    public static List<string> FindVersionsOfProgram(string programName)
    {
	    string programPath = Path.Combine(_programsPath, programName);
	    string mostRecentFilePath = Path.Combine(programPath, FindMostRecentFileInDirectory(programPath));

	    string? installationsPath = ScriptDataExtractor.GetProgramData(mostRecentFilePath).InstallationsPath;

	    return FindVersionSubdirectories(installationsPath!);
    }

    public static ProgramData GetProgramData(string programName, string versionName)
    {
	    string scriptPath = Path.Combine(_programsPath, programName, $"{versionName}.aisl");
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
    public static bool CheckDirectoryValidity(string? path)
    {
	    return !string.IsNullOrEmpty(path) && Directory.Exists(path) && ProgramService.FindVersionSubdirectories(path!).Count > 0;
    }
}