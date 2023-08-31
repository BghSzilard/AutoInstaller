using AISL;
using System.IO;

namespace Core;

public static class ProgramService
{
    private static readonly string _databasePath;
    private static readonly string _programsPath;
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

        //File.WriteAllText(filePath, AISLScriptBuilder.Build(programData));
        using StreamWriter writer = new(filePath);

        ProgramData mockData = new()
        {
            Name = "Simcenter Test Cloud Blueprint",
            InstallationsPath = "D:\\Siemens\\tcb",
            ParameterList = new()
            {
                new ParameterData()
                {
                    Type = ParameterType.number,
                    Name = "Port",
                    DefaultValue = "8080",
                },
                new ParameterData()
                {
                    Type = ParameterType.@string,
                    Name = "ServerName",
                },
                new ParameterData()
                {
                    Type = ParameterType.choice,
                    Name = "DropDown",
                    Options = new() { "option1", "option2" },
                },
                new ParameterData()
                {
                    Type = ParameterType.flag,
                    Name = "Tick",
                },
                new ParameterData()
                {
                    Type = ParameterType.@string,
                    Name = "FixedParameter",
                    FixedValue = "FixedValue"
                },
                new ParameterData()
                {
                    IsOptional = true,
                    Type = ParameterType.@string,
                    Name = "OptionalValue"
                }
            },
            InstallerPath = "D:\\Siemens\\tcb\\230822_1.1.9_core\\Simcenter Test Cloud Blueprint Setup.msi",
            Uninstall = true
        };

        writer.Write(AISLScriptBuilder.Build(programData));
    }

    public static List<string> FindSubdirectories(string directoryPath)
    {
        List<string> subdirectories = Directory.GetDirectories(directoryPath).ToList();
        return subdirectories;
    }

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
}