using AISL;
using System.IO;

namespace Core;

public static class ProgramService
{
    private static readonly string _databasePath;

    static ProgramService()
    {
        string databasePath = Enumerable.Range(0, 4).Aggregate(Environment.CurrentDirectory,
            (current, _) => Path.GetDirectoryName(current)!);
        _databasePath = Path.Combine(databasePath, "Database");

        if (!Directory.Exists(_databasePath))
        {
            Directory.CreateDirectory(_databasePath);
        }
    }

    public static void SaveProgram(ProgramData programData)
    {
        string programsPath = Path.Combine(_databasePath, "Programs");

        if (!Directory.Exists(programsPath))
        {
            Directory.CreateDirectory(programsPath);
        }

        string filePath = Path.Combine(programsPath, $"{programData.Version}.aisl");

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

        writer.Write(AISLScriptBuilder.Build(mockData));
    }

    public static List<string> FindVersionSubdirectories(string directoryPath)
    {
        List<string> versionDirectories = Directory.GetDirectories(directoryPath).ToList();

        versionDirectories.RemoveAll(directory => Directory.GetFiles(directory, "*.msi").Length == 0);

        List<string> versions = new();
        versionDirectories.ForEach(directory => versions.Add(directory.Replace(@$"{directoryPath}\", string.Empty)));

        return versions;
    }
}