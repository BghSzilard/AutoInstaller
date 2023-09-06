using AISL;
using Antlr4.Runtime;
using Core;
using System.Management.Automation.Language;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace AISLTesting;

public class AISLScriptBuilderTests
{
    [Fact]
    public void TestWithNoParameters()
    {
        string scriptTest = """
            FIND "Test" AT "C:\";
            UNINSTALL "Test";
            EXECUTE "C:\ceva" WITH installation_parameters;
            """;

        ProgramData programData = new ProgramData();

        programData.Name = "Test";
        programData.InstallationsPath = @"C:\";
        programData.InstallerPath = @"C:\ceva";
        string generatedScript = AISLScriptBuilder.Build(programData);

        string pattern = @"[\r\t]";

        // Use Regex.Replace to remove \n, \r, and \t from the generatedScript
        //generatedScript = Regex.Replace(generatedScript, pattern, string.Empty);
        scriptTest = Regex.Replace(generatedScript, pattern, string.Empty);

        Assert.Equal(scriptTest, generatedScript);
    }

    [Fact]
    public void TestWithFewParameters()
    {
        string scriptTest = """
            FIND "Simcenter Test Cloud Blueprint" AT "D:\Siemens\tcb";
            HAS (
            	number Port WITH DEFAULT 8080,
            	string ServerName WITH DEFAULT "C:\",
            	choice DropDown FROM ["option1", "option2"],
            ) AS installation_parameters;
            UNINSTALL "Simcenter Test Cloud Blueprint";
            EXECUTE "D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi" WITH installation_parameters;
            """;
        ProgramData programData = new ProgramData();

        programData.Name = "Simcenter Test Cloud Blueprint";
        programData.InstallationsPath = @"D:\Siemens\tcb";
        programData.InstallerPath = @"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi";
       

        var parameter1 = new ParameterData
        {
            Type = ParameterType.number,
            Name = "Port",
            Value = "8080",
            IsOptional = false,
            IsReadOnly = false,
            Options = null
        };

        var parameter2 = new ParameterData
        {
            Type = ParameterType.@string,
            Name = "ServerName",
            Value = @"C:\",
            IsOptional = false,
            IsReadOnly = false,
            Options = null
        };

        var parameter3 = new ParameterData
        {
            Type = ParameterType.choice,
            Name = "DropDown",
            Value = null,
            IsOptional = false,
            IsReadOnly = false,
            Options = new() { "option1", "option2" }
        };

        programData.ParameterList.Add(parameter1);
        programData.ParameterList.Add(parameter2);
        programData.ParameterList.Add(parameter3);

        string generatedScript = AISLScriptBuilder.Build(programData);

        // Define a regular expression pattern to match \n, \r, and \t
        string pattern = @"[\r\t]";

        // Use Regex.Replace to remove \n, \r, and \t from the generatedScript
        generatedScript = Regex.Replace(generatedScript, pattern, string.Empty);
        scriptTest = Regex.Replace(generatedScript, pattern, string.Empty);

        Assert.Equal(scriptTest, generatedScript);
    }

    [Fact]
    public void TestWithAllParameters()
    {
        string scriptTest = """
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
        ProgramData programData = new ProgramData();

        programData.Name = "Simcenter Test Cloud Blueprint";
        programData.InstallationsPath = @"D:\Siemens\tcb";
        programData.InstallerPath = @"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi";

        var parameter1 = new ParameterData
        {
            Type = ParameterType.number,
            Name = "Port",
            Value = "8080",
            IsOptional = false,
            IsReadOnly = false,
            Options = null
        };

        var parameter2 = new ParameterData
        {
            Type = ParameterType.@string,
            Name = "ServerName",
            Value = @"C:\",
            IsOptional = false,
            IsReadOnly = false,
            Options = null
        };

        var parameter3 = new ParameterData
        {
            Type = ParameterType.choice,
            Name = "DropDown",
            Value = null,
            IsOptional = false,
            IsReadOnly = false,
            Options = new() { "option1", "option2" }
        };
        
        var parameter4 = new ParameterData
        {
            Type = ParameterType.flag,
            Name = "Tick",
            Value = null,
            IsOptional = false,
            IsReadOnly = false,
            Options = null
        };

        var parameter5 = new ParameterData
        {
            Type = ParameterType.@string,
            Name = "FixedParameter",
            Value = "FixedValue",
            IsOptional = false,
            IsReadOnly = true,
            Options = null
        };
        
        var parameter6 = new ParameterData
        {
            Type = ParameterType.@string,
            Name = "OptionalValue",
            Value = null,
            IsOptional = true,
            IsReadOnly = false,
            Options = null
        };

        programData.ParameterList.Add(parameter1);
        programData.ParameterList.Add(parameter2);
        programData.ParameterList.Add(parameter3);
        programData.ParameterList.Add(parameter4);
        programData.ParameterList.Add(parameter5);
        programData.ParameterList.Add(parameter6);

        string generatedScript = AISLScriptBuilder.Build(programData);

        string pattern = @"[\r\t]";

        generatedScript = Regex.Replace(generatedScript, pattern, string.Empty);
        scriptTest = Regex.Replace(generatedScript, pattern, string.Empty);

        Assert.Equal(scriptTest, generatedScript);
    }
}
