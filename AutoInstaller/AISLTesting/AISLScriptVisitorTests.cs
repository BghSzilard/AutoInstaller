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
        new() { Type = "number", Name = "Port", DefaultValue = "8080", IsOptional = false, FixedValue = null, Options = null },
        new() { Type = "string", Name = "ServerName", DefaultValue = null, IsOptional = false, FixedValue = null, Options = null },
        new() { Type = "choice", Name = "DropDown", DefaultValue = null, IsOptional = false, FixedValue = null, Options = new() { "option1", "option2"} },
        new() { Type = "flag", Name = "Tick", DefaultValue = null, IsOptional = false, FixedValue = null, Options = null },
        new() { Type = "string", Name = "FixedParameter", DefaultValue = null, IsOptional = false, FixedValue = "FixedValue", Options = null },
        new() { Type = "string", Name = "OptionalValue", DefaultValue = null, IsOptional = true, FixedValue = null, Options = null }
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
    public void UninstallIsValid()
    {
        Assert.True(_programInfo.Uninstall);
    }

    [Fact]
    public void InstallerPathIsValid()
    {
        Assert.Equal(@"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi", _programInfo.InstallerPath);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void ParameterIsValid(int index)
    {
        Assert.Equal(ExpectedParameterList[index], _programInfo.ParameterList[index]);
    }
}