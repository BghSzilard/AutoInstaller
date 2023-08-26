using Antlr4.Runtime.Misc;
using static AISLParser;

namespace AISL;

public class AISLScriptVisitor : AISLBaseVisitor<ProgramInfo>
{
    private readonly ProgramInfo _programInfo = new();

    public override ProgramInfo VisitScript([NotNull] ScriptContext context)
    {
        VisitChildren(context);
        return _programInfo;
    }

    public override ProgramInfo VisitProgramName([NotNull] ProgramNameContext context)
    {
        _programInfo.Name = context.GetText().Trim('"');
        return _programInfo;
    }

    public override ProgramInfo VisitInstallationsPath([NotNull] InstallationsPathContext context)
    {
        _programInfo.InstallationsPath = context.GetText().Trim('"');
        return _programInfo;
    }

    public override ProgramInfo VisitParameter([NotNull] ParameterContext context)
    {
        ParameterInfo parameter = new()
        {
            Type = context.parameterType().GetText(),
            Name = context.parameterName().GetText()
        };

        if (context.parameterDefaultValue() != null)
        {
            parameter.DefaultValue = context.parameterDefaultValue().GetText();
        }

        _programInfo.ParameterList.Add(parameter);

        return _programInfo;
    }
}
