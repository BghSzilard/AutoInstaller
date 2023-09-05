using AISL;

namespace Core;

public static class AISLScriptBuilder
{
    private static string AddFindStatement(string script, ProgramData programData)
    {
        script += $@"FIND ""{programData.Name}"" AT ""{programData.InstallationsPath}"";";
        script += "\n";
        return script;
    }

    private static string AddParametersStatement(string script, ProgramData programData)
    {
        script += "HAS ( \n";
        foreach (var parameter in programData.ParameterList)
        {
            script += "\t";
            if (parameter.IsOptional)
            {
                script += "optional ";
            }
            script += $"{parameter.Type} {parameter.Name}";

            if (parameter.IsReadOnly == true)
            {
                if (parameter.Type == ParameterType.@string)
                {
                    script += $" = \"{parameter.Value}\"";
                }
                else
                {
                    script += $" = {parameter.Value}";
                }
            }

            if (parameter.Options != null)
            {
                script += " FROM [";
                foreach (var option in parameter.Options)
                {
                    script += $@"""{option}"", ";
                }
                script = script.Remove(script.Length - 2);
                script += "]";
            }

            if (parameter.Value != null)
            {
                if (parameter.Type == ParameterType.@string)
                {
                    script += $" WITH DEFAULT \"{parameter.Value}\"";
                }
                else
                {
                    script += $" WITH DEFAULT {parameter.Value}";
                }

            }
            script += ",\n";
        }
        script = script.Remove(script.Length - 2);
        script += "\n) AS installation_parameters;";
        return script;
    }

    private static string AddUninstallStatement(string script, ProgramData programData)
    {
        script += "\n";
        script += $@"UNINSTALL ""{programData.Name}"";";
        return script;
    }

    private static string AddExecuteStatement(string script, ProgramData programData)
    {
        script += "\n";
        script += $@"EXECUTE ""{programData.InstallerPath}"" WITH installation_parameters;";
        return script;
    }

    public static string Build(ProgramData programData)
    {
        string script = string.Empty;
        script = AddFindStatement(script, programData);
        if (programData.ParameterList.Count != 0)
        {
            script = AddParametersStatement(script, programData);
        }
        script = AddUninstallStatement(script, programData);
        script = AddExecuteStatement(script, programData);
        return script;
    }
}