using AISL;
using Microsoft.PowerShell.Commands;
using System.Diagnostics;
using System.Security.Claims;

namespace Core
{
    public static class PowershellScriptBuilder
    {
        public static string BuildPowershellInstallScript(ProgramData programData, string selectedVersion)
        {
            string absoluteExecutablePath = Path.Combine(programData.InstallationsPath, selectedVersion, programData.InstallerPath);
            string powershellScript = $"& \"{absoluteExecutablePath}\" ";
            foreach (var parameter in programData.ParameterList)
            {
                powershellScript += $"{parameter.Name}=\'\"{parameter.Value}\"\' ";
            }
            powershellScript += "/qb+";
            return powershellScript;
        }

        public static string BuildPowershellUninstallScript(string programName)
        {
            string powershellScript = $"$MyApp = Get-WmiObject -Class Win32_Product | Where-Object {{ $_.Name -eq '{programName}' }}";
            powershellScript += "\n$MyApp.Uninstall()";
            return powershellScript;
        }
        public static string BuildPowershelGetNameScript(string installerPath)
        {
            string powershellScript = $"$programName = (Get-Item '{installerPath}').VersionInfo.ProductName";
            powershellScript += "\n$programName";
            return powershellScript;
        }
    }
}