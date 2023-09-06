using AISL;
using Antlr4.Runtime;
using Core;
using System.Runtime.InteropServices;
using System.Text;
using Xunit.Abstractions;

namespace AISLTesting;

public class PowershellExecutorTests
{
    [Fact]
    public void RunPowershellInstallerTest()
    {
        ProgramData programData = new ProgramData();

        programData.InstallerPath = @"D:\Siemens\tcb\230822_1.1.9_core\Simcenter Test Cloud Blueprint Setup.msi";
        programData.InstallationsPath = @"D:\Siemens\tcb";

        var parameter1 = new ParameterData
        {
            Type = ParameterType.number,
            Name = "Port",
            Value = "8080",
            IsOptional = false,
            IsReadOnly = false,
            Options = null
        };

        programData.ParameterList.Add(parameter1);

        string selectedVersion = "230726_1.1.8_core";

        PowershellExecutor.RunPowershellInstaller(programData, selectedVersion);
    }

    [Fact]
    public void RunPowershellUninstallerAsyncTest()
    {
        ProgramData programData = new ProgramData();

        programData.Name = "Test";

        PowershellExecutor.RunPowershellUninstallerAsync(programData.Name);
    }
}


