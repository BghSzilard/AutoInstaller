using AISL;
using System.Diagnostics;

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
        public static void RunPowershellInstaller(ProgramData programData)
        {
            Process process = new();
            InitializeProcess(process);
            string powershellScript = PowershellScriptBuilder.BuildPowershellInstallScript(programData);

            process.Start();
            process.StandardInput.WriteLine(powershellScript);
            process.StandardInput.Close();

            process.WaitForExit();
        }
        
        public static Task RunPowershellUninstallerAsync(string programName)
        {
            Process process = new Process();
            InitializeProcess(process);
            string powershellScript = PowershellScriptBuilder.BuildPowershellUninstallScript(programName);

            process.Start();
            process.StandardInput.WriteLine(powershellScript);
            process.StandardInput.Close();

            return process.WaitForExitAsync();
        }
        public static string RunPowershellGetNameScript(string installerPath)
        {
            Process process = new Process();
            InitializeProcess(process);
            process.StartInfo.Arguments = "-NoLogo -NoProfile -NonInteractive -Command -";
            string powershellScript = PowershellScriptBuilder.BuildPowershelGetNameScript(installerPath);

            process.Start();
            process.StandardInput.WriteLine(powershellScript);
            process.StandardInput.Close();
            string output = process.StandardOutput.ReadToEnd()
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty);

            process.WaitForExit();
            return output;
        }
    }
}
