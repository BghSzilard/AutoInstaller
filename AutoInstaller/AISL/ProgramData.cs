﻿namespace AISL;

public enum ParameterType
{
    number,
    @string,
    choice,
    flag
}

// todo: simplify logic by having a single Value property and a bool that checks if the user wants a ReadOnly parameter Value (i.e. FixedValue) or ReadWrite (i.e. DefaultValue)
public class ParameterData
{
    public bool IsOptional { get; set; }
    public ParameterType Type { get; set; }
    public string Name { get; set; }
    public string? DefaultValue { get; set; }
    public string? FixedValue { get; set; }
    public List<string>? Options { get; set; }
    public string? Value { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ParameterData info &&
               IsOptional == info.IsOptional &&
               Type == info.Type &&
               Name == info.Name &&
               DefaultValue == info.DefaultValue &&
               FixedValue == info.FixedValue &&
               (Options == null && Options == info.Options || Options != null && info.Options != null && Options.SequenceEqual(info.Options));
    }
}

public class ProgramData
{
    public string? Name { get; set; }
    public string? InstallationsPath { get; set; }
    public List<ParameterData> ParameterList { get; set; } = new();
    public bool Uninstall { get; set; }
    public string? InstallerPath { get; set; } // should it be here?

    public string? Version { get; set; }

    public string? PathToInvokeAt { get; set; }
    public string? InvokeBlock { get; set; }
}
