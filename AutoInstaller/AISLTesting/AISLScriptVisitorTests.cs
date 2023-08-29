using AISL;
using Antlr4.Runtime;
using System.Text;

namespace AISLTesting;

public class AISLScriptVisitorTests
{
    private readonly ProgramData _programData;

    private const string ScriptPath = @"D:\Siemens\AutoInstaller\AutoInstaller\AISL\script.aisl";

    private List<ParameterData> ExpectedParameterList = new()
    {
        new() { Type = ParameterType.number, Name = "Port", DefaultValue = "8080", IsOptional = false, FixedValue = null, Options = null },
        new() { Type = ParameterType.@string, Name = "ServerName", DefaultValue = null, IsOptional = false, FixedValue = null, Options = null },
        new() { Type = ParameterType.choice, Name = "DropDown", DefaultValue = null, IsOptional = false, FixedValue = null, Options = new() { "option1", "option2"} },
        new() { Type = ParameterType.flag, Name = "Tick", DefaultValue = null, IsOptional = false, FixedValue = null, Options = null },
        new() { Type = ParameterType.@string, Name = "FixedParameter", DefaultValue = null, IsOptional = false, FixedValue = "FixedValue", Options = null },
        new() { Type = ParameterType.@string, Name = "OptionalValue", DefaultValue = null, IsOptional = true, FixedValue = null, Options = null }
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
        _programData = visitor.Visit(scriptContext);
    }

    [Fact]
    public void ProgramNameIsValid()
    {
        Assert.Equal("Simcenter Test Cloud Blueprint", _programData.Name);
    }

    [Fact]
    public void InstallationsPathIsValid()
    {
        Assert.Equal(@"D:\Siemens\tcb", _programData.InstallationsPath);
    }

    [Fact]
    public void UninstallIsValid()
    {
        Assert.True(_programData.Uninstall);
    }

    [Fact]
    public void InstallerPathIsValid()
    {
        Assert.Equal(@"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi", _programData.InstallerPath);
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
        Assert.Equal(ExpectedParameterList[index], _programData.ParameterList[index]);
    }
}