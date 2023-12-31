﻿namespace AISL;

public enum ParameterType
{
    number,
    @string,
    choice,
    flag
}

public class ParameterData
{
    public bool IsOptional { get; set; }
    public ParameterType Type { get; set; }
    public string Name { get; set; }
    public string? Value { get; set; }
    public bool IsReadOnly { get; set; }
    public List<string>? Options { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ParameterData info &&
               IsOptional == info.IsOptional &&
               Type == info.Type &&
               Name == info.Name &&
               Value == info.Value &&
               IsReadOnly == info.IsReadOnly &&
               (Options == null && Options == info.Options || Options != null && info.Options != null && Options.SequenceEqual(info.Options));
    }
}

public class ProgramData
{
    public string? Name { get; set; }
    public string? InstallationsPath { get; set; }
    public List<ParameterData> ParameterList { get; set; } = new();
    public string? InstallerPath { get; set; }
    public string? PathToInvokeInstallAt { get; set; }
    public string? InvokeInstallBlock { get; set; }
    public string? PathToInvokeUninstallAt { get; set; }
    public string? InvokeUninstallBlock { get; set; }
}
