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

    public override ProgramData VisitFindInstruction([NotNull] FindInstructionContext context)
    {
        var quotedTextContexts = context.QUOTED_TEXT();
        _programData.Name = quotedTextContexts[0].GetText().Trim('"');
        _programData.InstallationsPath = quotedTextContexts[1].GetText().Trim('"');
        return _programData;
    }

    public override ProgramData VisitNonChoiceParameter([NotNull] NonChoiceParameterContext context)
    {
        ParameterData parameter = new()
        {
            IsOptional = context.OPTIONAL() != null,
            Name = context.WORD().GetText(),
            IsReadOnly = false
        };

        string type = context.TYPE().GetText();
        switch (type)
        {
            case "string":
                parameter.Type = ParameterType.@string;
                break;
            case "number":
                parameter.Type = ParameterType.number;
                break;
            case "flag":
                parameter.Type = ParameterType.flag;
                break;
            default:
                throw new InvalidDataException("Unrecognized type");
        }

        if (context.defaultOrFixed() != null && context.defaultOrFixed().defaultParamValue() != null)
        {
            parameter.Value = context.defaultOrFixed().defaultParamValue().valueOrString().GetText().Trim('"');
        }

        if (context.defaultOrFixed() != null && context.defaultOrFixed().fixedParamValue() != null)
        {
            parameter.Value = context.defaultOrFixed().fixedParamValue().valueOrString().GetText().Trim('"');
            parameter.IsReadOnly = true;
        }

        _programData.ParameterList.Add(parameter);

        return _programData;
    }

    public override ProgramData VisitChoiceParameter([NotNull] ChoiceParameterContext context)
    {
        ParameterData parameter = new()
        {
            IsOptional = context.OPTIONAL() != null,
            Name = context.WORD().GetText(),
            Type = ParameterType.choice,
            IsReadOnly = false,
            Options = new()
        };

        var options = context.optionList().valueOrString().ToList();
        options.ForEach(option => parameter.Options.Add(option.GetText().Trim(' ').Trim('"')));

        if (context.defaultOrFixed() != null && context.defaultOrFixed().defaultParamValue() != null)
        {
            parameter.Value = context.defaultOrFixed().defaultParamValue().valueOrString().GetText().Trim('"');
        }

        if (context.defaultOrFixed() != null && context.defaultOrFixed().fixedParamValue() != null)
        {
            parameter.Value = context.defaultOrFixed().fixedParamValue().valueOrString().GetText().Trim('"');
            parameter.IsReadOnly = true;
        }

        _programData.ParameterList.Add(parameter);

        return _programData;
    }

    public override ProgramData VisitExecuteInstruction([NotNull] ExecuteInstructionContext context)
    {
        _programData.InstallerPath = context.QUOTED_TEXT().GetText().Trim('"');
        return _programData;
    }

    public override ProgramData VisitInvokeInstallInstruction([NotNull] InvokeInstallInstructionContext context)
    {
        _programData.InvokeInstallBlock = context.anything()
            .GetText()
            .Trim('\r')
            .Trim('\n')
            .Replace(@"\}", "}")
            .Replace(@"\{", "{");

        _programData.PathToInvokeInstallAt = context.QUOTED_TEXT().GetText().Trim('"');
        return _programData;
    }

    public override ProgramData VisitInvokeUninstallInstruction([NotNull] InvokeUninstallInstructionContext context)
    {
        _programData.InvokeUninstallBlock = context.anything()
           .GetText()
           .Trim('\r')
           .Trim('\n')
           .Replace(@"\}", "}")
           .Replace(@"\{", "{");

        _programData.PathToInvokeUninstallAt = context.QUOTED_TEXT().GetText().Trim('"');
        return _programData;
    }
}