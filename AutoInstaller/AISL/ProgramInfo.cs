namespace AISL;

public struct ParameterInfo
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string DefaultValue { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ParameterInfo info &&
               Type == info.Type &&
               Name == info.Name &&
               DefaultValue == info.DefaultValue;
    }
}

public class ProgramInfo
{
    public string Name { get; set; } = "";
    public string InstallationsPath { get; set; } = "";
    public List<ParameterInfo> ParameterList { get; set; } = new();
}
