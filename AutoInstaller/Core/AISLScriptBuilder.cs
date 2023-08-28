namespace Core;

public class AISLScriptBuilder
{
    public string BuildScript(ProgramData programData)
    {
        string script = $@"FIND ""{programData.Name}"" AT ""{programData.InstallationsPath}""";
        if (programData.Parameters.Count != 0 )
        {
            script += "HAS ( \n";
        }
        return script;
    }
}