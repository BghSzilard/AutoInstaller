using AISL;

namespace Core;

public static class AISLScriptBuilder
{
    public static string Build(ProgramData programData)
    {
        string script = $@"FIND ""{programData.Name}"" AT ""{programData.InstallationsPath}""";
        return script;
    }
}