using AISL;
using Antlr4.Runtime;
using System.Text;

namespace AISLTesting;

public class AISLScriptVisitorTests
{
    private readonly ProgramInfo _programInfo;

    private const string ScriptPath = @"D:\Siemens\AutoInstaller\AutoInstaller\AISL\script.aisl";

    private List<ParameterInfo> ExpectedParameterList = new()
    {
        new() { Type = "number", Name = "Port", DefaultValue = "8080"},
        new() { Type = "string", Name = "ServerName", DefaultValue = null!},
        new() { Type = "flag", Name = "Tick", DefaultValue = null!}
    };

    public AISLScriptVisitorTests()
    {
        string scriptText = File.ReadAllText(ScriptPath, Encoding.UTF8);

        AntlrInputStream inputStream = new AntlrInputStream(scriptText);
        AISLLexer aislLexer = new AISLLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
        AISLParser aislParser = new AISLParser(commonTokenStream);

        AISLParser.ScriptContext scriptContext = aislParser.script();
        AISLScriptVisitor visitor = new();
        _programInfo = visitor.Visit(scriptContext);
    }

    [Fact]
    public void ProgramNameIsValid()
    {
        Assert.Equal("Simcenter Test Cloud Blueprint", _programInfo.Name);
    }

    [Fact]
    public void InstallationsPathIsValid()
    {
        Assert.Equal(@"D:\Siemens\tcb", _programInfo.InstallationsPath);
    }

    [Fact]
    public void ParameterListIsValid()
    {
        for (int i = 0; i < ExpectedParameterList.Count; i++)
        {
            Assert.Equal(ExpectedParameterList[i], _programInfo.ParameterList[i]);
        }
    }
}