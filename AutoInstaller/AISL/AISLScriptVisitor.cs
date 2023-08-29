using Antlr4.Runtime.Misc;
using static AISLParser;

namespace AISL;

public class AISLScriptVisitor : AISLBaseVisitor<ProgramData>
{
    private readonly ProgramData _programData = new();

    public override ProgramData VisitScript([NotNull] ScriptContext context)
    {
        VisitChildren(context);
        return _programData;
    }

    public override ProgramData VisitUninstallInstruction([NotNull] UninstallInstructionContext context)
    {
        _programData.Uninstall = true;
        return _programData;
    }

    public override ProgramData VisitExecuteInstruction([NotNull] ExecuteInstructionContext context)
    {
        _programData.InstallerPath = context.installerPath().GetText().Trim('"');
        return _programData;
    }

    public override ProgramData VisitProgramName([NotNull] ProgramNameContext context)
    {
        _programData.Name = context.GetText().Trim('"');
        return _programData;
    }

    public override ProgramData VisitInstallationsPath([NotNull] InstallationsPathContext context)
    {
        _programData.InstallationsPath = context.GetText().Trim('"');
        return _programData;
    }

    public override ProgramData VisitParameter([NotNull] ParameterContext context)
    {
        ParameterData parameter = new()
        {
            Type = (ParameterType)Enum.Parse(typeof(ParameterType), context.parameterType().GetText()),
            Name = context.parameterName().GetText()
        };

        if (context.parameterDefaultValue() != null)
        {
            parameter.DefaultValue = context.parameterDefaultValue().GetText();
        }

        if (context.parameterIsOptional() != null)
        {
            parameter.IsOptional = true;
        }

        if (context.optionList() != null)
        {
            parameter.Options = context.optionList().option().Select(optionContext => optionContext.GetText().Trim('"')).ToList();
        }

        if (context.parameterFixedValue() != null)
        {
            parameter.FixedValue = context.parameterFixedValue().GetText().Trim('"');
        }

        _programData.ParameterList.Add(parameter);

        return _programData;
    }
}