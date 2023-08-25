namespace AISL;

public struct ParameterInfo
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string DefaultValue { get; set; }
}

public class ProgramInfo
{
    public string Name { get; set; } = "";
    public string InstallationsPath { get; set; } = "";
    public List<ParameterInfo> Parameters { get; set; } = new();
}
