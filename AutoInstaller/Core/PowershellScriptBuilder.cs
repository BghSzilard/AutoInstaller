using AISL;
using Microsoft.PowerShell.Commands;
using System.Diagnostics;
using System.Security.Claims;

namespace Core;

public static class PowershellScriptBuilder
{
    public static string BuildPowershellInstallScript(ProgramData programData, string selectedVersion, bool logToFile)
    {
        string powershellScript = "";
        if (programData.InvokeUninstallBlock != null)
        {
            string uninstallInvokePath = "";
            if (ProgramService.IsPathAbsolute(programData.PathToInvokeUninstallAt))
            {
                uninstallInvokePath = programData.PathToInvokeUninstallAt;
            }
            else
            {
                uninstallInvokePath = Path.Combine(programData.InstallationsPath, selectedVersion, uninstallInvokePath);
            }
            powershellScript += $"cd {uninstallInvokePath}\n";
            powershellScript += programData.InvokeUninstallBlock;
        }
        if (programData.InvokeInstallBlock == null)
        {
            string installsPath = Enumerable.Range(0, 4).Aggregate(Environment.CurrentDirectory,
            (current, _) => Path.GetDirectoryName(current)!);

            installsPath = Path.Combine(installsPath, "Installs", programData.Name);
            if (!Directory.Exists(installsPath))
            {
                Directory.CreateDirectory(installsPath);
            }
            string absoluteExecutablePath = Path.Combine(programData.InstallationsPath, selectedVersion, programData.InstallerPath);
            powershellScript = $"& \"{absoluteExecutablePath}\" ";
           
            if (logToFile)
            {
                powershellScript += "/Lp ";
                powershellScript += $"\"{Path.Combine(installsPath, "installLog.txt")}\" ";
            }
            foreach (var parameter in programData.ParameterList)
            {
                powershellScript += $"{parameter.Name}=\'\"{parameter.Value}\"\' ";
            }
            powershellScript += "/qb+";
        }
        else
        {
            string invokeInstallPath = "";
            if (ProgramService.IsPathAbsolute(programData.PathToInvokeInstallAt))
            {
                invokeInstallPath = programData.PathToInvokeInstallAt;
            }
            else
            {
                invokeInstallPath = Path.Combine(programData.InstallationsPath, selectedVersion, invokeInstallPath);
            }
            powershellScript += $"cd {invokeInstallPath}\n";
            powershellScript += programData.InvokeInstallBlock;
        }
        return powershellScript;
    }

    public static string BuildPowershellUninstallScript(string productCode)
    {
        //string powershellScript = $"$MyApp = Get-WmiObject -Class Win32_Product | Where-Object {{ $_.Name -eq '{programName}' }}";
        //powershellScript += "\n$MyApp.Uninstall()";
        //return powershellScript;

        string powershellScript = $"Start-Process -FilePath \"cmd.exe\" -ArgumentList \'/c msiexec /x {productCode} /passive\' -Wait";
        return powershellScript;
    }
}