namespace AISL;

public struct ParameterInfo
{
    public bool IsOptional { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string? DefaultValue { get; set; }
    public string? FixedValue { get; set; }
    public List<string>? Options { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ParameterInfo info &&
               IsOptional == info.IsOptional &&
               Type == info.Type &&
               Name == info.Name &&
               DefaultValue == info.DefaultValue &&
               FixedValue == info.FixedValue &&
               (Options == null && Options == info.Options || Options != null && info.Options != null && Options.SequenceEqual(info.Options));
    }
}

public class ProgramInfo
{
    public string Name { get; set; } = "";
    public string InstallationsPath { get; set; } = "";
    public List<ParameterInfo> ParameterList { get; set; } = new();
    public bool Uninstall { get; set; }
    public string InstallerPath { get; set; } = ""; // should it be here?
}
