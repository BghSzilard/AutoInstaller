using AISL;
using Microsoft.Win32;
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

    public static List<string> GetAllProgramsFromComputer()
    {
        List<string> programNames = new List<string>();
        string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
        {
            foreach (string subkey_name in key.GetSubKeyNames())
            {
                using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                {
                    programNames.Add((string)subkey.GetValue("DisplayName"));
                }
            }
        }
        return programNames;
    }
    public static string? GetInstalledProgramNameFromInstaller(string installerPath)
    {
        if (installerPath.EndsWith(".msi"))
        {
            installerPath = FindExePath(installerPath);
        }
        string programToInstall = GetProgramName(installerPath);
        foreach (var installedProgram in GetAllProgramsFromComputer())
        {
            if (programToInstall == installedProgram)
            {
                return programToInstall;
            }
        }
        return null;
    }
    private static string FindExePath(string msiPath)
    {
        string msiDirectory = Path.GetDirectoryName(msiPath);
        if (msiDirectory != null)
        {
            var exePath = Directory.GetFiles(msiDirectory, "*.exe");
            return exePath[0];
        }
        throw new Exception("Could not find .exe");
    }
    private static string GetProgramName(string installerPath)
    {
        return PowershellExecutor.RunPowershellGetNameScript(installerPath);
    }

    public static string GetMainApplicationFolder(string? installerPath)
    {
	    DirectoryInfo installerDirInfo = new DirectoryInfo(installerPath!);

	    return installerDirInfo.Parent.Parent.Parent.FullName;
    }

    public static string GetApplicationVersion(string? installerPath)
    {
	    DirectoryInfo installerDirInfo = new DirectoryInfo(installerPath!);

	    return installerDirInfo.Parent.Parent.Name;
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
}