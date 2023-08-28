namespace Core;

public class AISLScriptBuilder
{
    public void BuildScript(ProgramData programData)
    {
        string script = $@"FIND ""{programData.Name}"" AT ""{programData.InstallationsPath}""";
        
    }
}