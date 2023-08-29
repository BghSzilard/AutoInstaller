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

        if (Directory.Exists(_databasePath))
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
        writer.Write(AISLScriptBuilder.Build(programData));
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