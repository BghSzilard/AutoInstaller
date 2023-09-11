using AISL;
using Antlr4.Runtime;
using Core;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit.Abstractions;

namespace AISLTesting;

public class AISLPowershellScriptBuilderTests
{
    [Fact]
    public void BuildPowershellInstallScriptTest()
    {
        string scriptTest = "& \"D:\\Siemens\\tcb\\230822_1.1.9_core\\Simcenter Test Cloud Blueprint Setup.msi\" Port='\"8080\"' /qb+";
        ProgramData programData = new ProgramData();

        programData.InstallerPath = @"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi";
        programData.InstallationsPath = @"D:\Siemens\tcb";
        var parameter1 = new ParameterData
        {
            Type = ParameterType.number,
            Name = "Port",
            IsOptional = false,
            Options = null,
            Value = "8080"
        };

        programData.ParameterList.Add(parameter1);

        string selectedVersion = "230822_1.1.9_core";

        string generatedScript = PowershellScriptBuilder.BuildPowershellInstallScript(programData, selectedVersion, true);

        Assert.Equal(scriptTest, generatedScript);
    }

    [Fact]
    public void BuildPowershellUninstallScriptTest()
    {
        ProgramData programData = new ProgramData();

        programData.Name = "Test";

        string scriptTest = "$MyApp = Get-WmiObject -Class Win32_Product | Where-Object { $_.Name -eq 'Test' }\n$MyApp.Uninstall()";

        string generatedScript = PowershellScriptBuilder.BuildPowershellUninstallScript(programData.Name);

        Assert.Equal(scriptTest, generatedScript);
    }
}
