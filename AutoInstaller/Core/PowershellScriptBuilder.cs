using AISL;
using Microsoft.PowerShell.Commands;
using System.Diagnostics;
using System.Security.Claims;

namespace Core
{
    public static class PowershellScriptBuilder
    {
        public static string BuildPowershellInstallScript(ProgramData programData, string selectedVersion, bool logToFile)
        {
	        string installsPath = Enumerable.Range(0, 4).Aggregate(Environment.CurrentDirectory,
		        (current, _) => Path.GetDirectoryName(current)!);

	        installsPath = Path.Combine(installsPath, "Installs", programData.Name);
	        if (!Directory.Exists(installsPath))
	        {
		        Directory.CreateDirectory(installsPath);
	        }

	        string absoluteExecutablePath = Path.Combine(programData.InstallationsPath, selectedVersion, programData.InstallerPath);
            string powershellScript = $"& \"{absoluteExecutablePath}\" ";
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
        public static string BuildPowershelGetNameScript(string installerPath)
        {
            string powershellScript = $"$programName = (Get-Item '{installerPath}').VersionInfo.ProductName";
            powershellScript += "\n$programName";
            return powershellScript;
        }
    }
}