using AISL;
using Antlr4.Runtime;
using System.Text;
using Xunit.Abstractions;

namespace AISLTesting;

public class AISLScriptVisitorTests
{
	private readonly ITestOutputHelper output;

    private readonly ProgramData _programData;

    private const string ScriptPath = @"C:\Users\Botond\Documents\Siemens\Summer School\Final\AutoInstaller\AutoInstaller\AISL\script.aisl";

    private List<ParameterData> ExpectedParameterList = new()
    {
        new() { Type = "number", Name = "Port", DefaultValue = "8080", IsOptional = false, FixedValue = null, Options = null },
        new() { Type = "string", Name = "ServerName", DefaultValue = null, IsOptional = false, FixedValue = null, Options = null },
        new() { Type = "choice", Name = "DropDown", DefaultValue = null, IsOptional = false, FixedValue = null, Options = new() { "option1", "option2"} },
        new() { Type = "flag", Name = "Tick", DefaultValue = null, IsOptional = false, FixedValue = null, Options = null },
        new() { Type = "string", Name = "FixedParameter", DefaultValue = null, IsOptional = false, FixedValue = "FixedValue", Options = null },
        new() { Type = "string", Name = "OptionalValue", DefaultValue = null, IsOptional = true, FixedValue = null, Options = null }
    };

    public AISLScriptVisitorTests(ITestOutputHelper output)
    {
	    this.output = output;

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

    public void ScriptWithIntentionalError_MissingLineOneSemicolon()
    {
	    string scriptText = """
            FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb"
            HAS (
            	number Port WITH DEFAULT 8080,
            	string ServerName,
            	choice DropDown FROM ["option1", "option2"],
            	flag Tick,
            	string FixedParameter = "FixedValue",
            	optional string OptionalValue
            ) AS installation_parameters;
            UNINSTALL "Simcenter Test Cloud Blueprint";
            EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
            
            """;

	    AntlrInputStream inputStream = new AntlrInputStream(scriptText);
	    AISLLexer aislLexer = new AISLLexer(inputStream);
	    CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
	    AISLParser aislParser = new AISLParser(commonTokenStream);
        aislParser.AddErrorListener(new SyntaxErrorListener());
	    //var errorHandler = aislParser.ErrorHandler;

	    AISLParser.ScriptContext scriptContext = aislParser.script();
	    AISLScriptVisitor visitor = new();
        _ = visitor.Visit(scriptContext);

    }

    [Fact]
    public void ThrowsExceptionOnSyntaxError()
    {
	    Exception exception = Assert.Throws<Exception>(ScriptWithIntentionalError_MissingLineOneSemicolon);

        output.WriteLine(exception.Message);
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