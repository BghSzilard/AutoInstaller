using AISL;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core;

public static class ProgramDataSerializer
{
    public static void SerializeToFile(ProgramData programData, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        string jsonString = JsonSerializer.Serialize(programData, options);
        File.WriteAllText(filePath, jsonString);
    }

    public static ProgramData? DeserializeFromFile(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        ProgramData? programData = JsonSerializer.Deserialize<ProgramData>(jsonString, options);
        return programData;
    }
}
