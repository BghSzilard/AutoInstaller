using AISL;
using Antlr4.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using Xunit.Abstractions;

namespace AISLTesting;

public class AISLScriptVisitorTests
{
	private readonly ITestOutputHelper output;

    private readonly ProgramData _programData;

    //private const string ScriptPath = @"C:\Users\Rares\Dropbox\My PC (DESKTOP-P06UPBI)\Desktop\SummerSchooInstaller\AutoInstaller\AutoInstaller\AISL\script.aisl";

    private List<ParameterData> ExpectedParameterList = new()
    {
        new() { Type = ParameterType.number, Name = "Port", Value = "8080", IsOptional = false, IsReadOnly = false, Options = null },
        new() { Type = ParameterType.@string, Name = "ServerName", Value = @"C:\", IsOptional = false, IsReadOnly = false, Options = null },
        new() { Type = ParameterType.choice, Name = "DropDown", Value = null, IsOptional = false, IsReadOnly = false, Options = new() { "option1", "option2"} },
        new() { Type = ParameterType.flag, Name = "Tick", Value = null, IsOptional = false, IsReadOnly = false, Options = null },
        new() { Type = ParameterType.@string, Name = "FixedParameter", Value = "FixedValue", IsOptional = false, IsReadOnly = true, Options = null },
        new() { Type = ParameterType.@string, Name = "OptionalValue", Value = null, IsOptional = true, IsReadOnly = false, Options = null }
    };

    public AISLScriptVisitorTests(ITestOutputHelper output)
    {
	    this.output = output;

        string scriptText = """
            FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
            HAS (
                number Port WITH DEFAULT 8080,
                string ServerName WITH DEFAULT "C:\",
                choice DropDown FROM ["option1", "option2"],
                flag Tick,
                string FixedParameter = "FixedValue",
                OPTIONAL string OptionalValue,
            ) AS installation_parameters;
            UNINSTALL "Simcenter Test Cloud Blueprint";
            EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
            INVOKE AS INSTALL {
                Copy-Item "whatever" "wherever"
                Copy-Item "whatever" | \{ "wherever" \}
            } AT "relative\path";
            INVOKE AS UNINSTALL {
                Copy-Item "whatever" "wherever" to uninstall
                Copy-Item "whatever" | \{ "wherever" \}
            } AT "relative\path";

            """;

        AntlrInputStream inputStream = new AntlrInputStream(scriptText);
        AISLLexer aislLexer = new AISLLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
        AISLParser aislParser = new AISLParser(commonTokenStream);

        AISLParser.ScriptContext scriptContext = aislParser.script();
        AISLScriptVisitor visitor = new();
        _programData = visitor.Visit(scriptContext);
    }

    [Theory]
    [InlineData("Simcenter Test Cloud Blueprint", @"D:\Siemens\tcb", @"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi")]
    public void ProgramDataPropertiesAreValid(
    string expectedName,
    string expectedInstallationsPath,
    string expectedInstallerPath)
    {
        Assert.Equal(expectedName, _programData.Name);
        Assert.Equal(expectedInstallationsPath, _programData.InstallationsPath);
        Assert.Equal(expectedInstallerPath, _programData.InstallerPath);
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

    public void DefaultScript()
    {
        string scriptText = """
            FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
            HAS (
            	number Port WITH DEFAULT 8080,
            	string ServerName WITH DEFAULT "C:\",
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

        AISLParser.ScriptContext scriptContext = aislParser.script();
        AISLScriptVisitor visitor = new();
        _ = visitor.Visit(scriptContext);
    }
    [Fact]
    public void InitialTest() 
    {
        var exception = Record.Exception(() => DefaultScript());
        Assert.Null(exception);
    }

    //public void ScriptErrorMissingOpenBracket()
    //{
    //    string scriptText = """
    //        FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
    //        HAS 
    //        	number Port WITH DEFAULT 8080,
    //        	string ServerName WITH DEFAULT "C:\",
    //        	choice DropDown FROM ["option1", "option2"],
    //        	flag Tick,
    //        	string FixedParameter = "FixedValue",
    //        	optional string OptionalValue
    //        ) AS installation_parameters;
    //        UNINSTALL "Simcenter Test Cloud Blueprint";
    //        EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;


    //        """;

    //    AntlrInputStream inputStream = new AntlrInputStream(scriptText);
    //    AISLLexer aislLexer = new AISLLexer(inputStream);
    //    CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
    //    AISLParser aislParser = new AISLParser(commonTokenStream);
    //    aislParser.AddErrorListener(new SyntaxErrorListener());

    //    AISLParser.ScriptContext scriptContext = aislParser.script();
    //    AISLScriptVisitor visitor = new();
    //    _ = visitor.Visit(scriptContext);
    //}
    //[Fact]
    //public void OpenBracketError()
    //{
    //    Exception exception = Assert.Throws<Exception>(OpenBracketError);

    //    output.WriteLine(exception.Message);
    //}
    [Fact]
    public void ThrowsExceptionOnSyntaxError_MissingParenthesis()
    {
        string scriptText = """
        FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
        HAS 
        	number Port WITH DEFAULT 8080,
        	string ServerName WITH DEFAULT "C:\",
        	choice DropDown FROM ["option1", "option2"],
        	flag Tick,
        	string FixedParameter = "FixedValue",
        	optional string OptionalValue
        ) AS installation_parameters;
        UNINSTALL "Simcenter Test Cloud Blueprint";
        EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
        
        """;

    Exception exception = Assert.Throws<Exception>(() => ParseScript(scriptText));

        output.WriteLine(exception.Message);
    }

    private void ParseScript(string scriptText)
    {
        AntlrInputStream inputStream = new AntlrInputStream(scriptText);
        AISLLexer aislLexer = new AISLLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
        AISLParser aislParser = new AISLParser(commonTokenStream);
        aislParser.AddErrorListener(new SyntaxErrorListener());

        AISLParser.ScriptContext scriptContext = aislParser.script();
        AISLScriptVisitor visitor = new AISLScriptVisitor();
        _ = visitor.Visit(scriptContext);
    }

    [Fact]
    public void MissingAS()
    {
        string scriptText = """
        FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
        HAS (
        	string ServerName WITH DEFAULT "C:\",
        	choice DropDown FROM ["option1", "option2"],
        	flag Tick,
        	string FixedParameter = "FixedValue",
        	optional string OptionalValue
        )  installation_parameters;
        UNINSTALL "Simcenter Test Cloud Blueprint";
        EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
        
        """;

        Exception exception = Assert.Throws<Exception>(() => ParseScript(scriptText));

        output.WriteLine(exception.Message);
    }

    public void MissingUninstallScript()
    {
        string scriptText = """
            FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
            HAS (
            	number Port WITH DEFAULT 8080,
            	string ServerName WITH DEFAULT "C:\",
            	choice DropDown FROM ["option1", "option2"],
            	flag Tick,
            	string FixedParameter = "FixedValue",
            	optional string OptionalValue
            ) AS installation_parameters;
            EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
            """;


        AntlrInputStream inputStream = new AntlrInputStream(scriptText);
        AISLLexer aislLexer = new AISLLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
        AISLParser aislParser = new AISLParser(commonTokenStream);

        AISLParser.ScriptContext scriptContext = aislParser.script();
        AISLScriptVisitor visitor = new();
        _ = visitor.Visit(scriptContext);
    }
    [Fact]
    public void MissingUnistallTest()
    {
        var exception = Record.Exception(() => MissingUninstallScript());
        Assert.Null(exception);
    }

    public void MissingHasAndUninstallScript()
    {
        string scriptText = """
            FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
            AS installation_parameters;
            EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
            """;


        AntlrInputStream inputStream = new AntlrInputStream(scriptText);
        AISLLexer aislLexer = new AISLLexer(inputStream);
        CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
        AISLParser aislParser = new AISLParser(commonTokenStream);

        AISLParser.ScriptContext scriptContext = aislParser.script();
        AISLScriptVisitor visitor = new();
        _ = visitor.Visit(scriptContext);
    }
    [Fact]
    public void MissingHasAndUnistallTest()
    {
        var exception = Record.Exception(() => MissingHasAndUninstallScript());
        Assert.Null(exception);
    }

    [Fact]
    public void OptionsMissingInsideSquareBrackets()
    {
        string scriptText = """
        FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
        HAS (
        	number Port WITH DEFAULT 8080,
        	string ServerName WITH DEFAULT "C:\",
        	choice DropDown FROM [],
        	flag Tick,
        	string FixedParameter = "FixedValue",
        	optional string OptionalValue
        ) AS installation_parameters;
        UNINSTALL "Simcenter Test Cloud Blueprint";
        EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
        """;



        Exception exception = Assert.Throws<Exception>(() => ParseScript(scriptText));

        output.WriteLine(exception.Message);
    }

}