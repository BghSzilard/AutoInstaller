using AISL;
using System.Diagnostics;
using System.Management.Automation;

namespace Core
{
    public static class PowershellExecutor
    {
        private static void InitializeProcess(Process process)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.StartInfo = psi;
        }
        public static void RunPowershellInstaller(ProgramData programData, string selectedVersion, bool logToFile)
        {
            Process process = new();
            InitializeProcess(process);
            string powershellScript = PowershellScriptBuilder.BuildPowershellInstallScript(programData, selectedVersion, logToFile);

            process.Start();
            process.StandardInput.WriteLine(powershellScript);
            process.StandardInput.Close();

            process.WaitForExit();
        }

        public static Task RunPowershellUninstallerAsync(string productCode)
        {
            Process process = new Process();
            InitializeProcess(process);

            string powershellScript = PowershellScriptBuilder.BuildPowershellUninstallScript(productCode);

            process.Start();
            process.StandardInput.WriteLine(powershellScript);
            process.StandardInput.Close();

            return process.WaitForExitAsync();
        }
    }
}
