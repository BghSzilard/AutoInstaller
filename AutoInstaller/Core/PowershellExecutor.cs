using AISL;
using System.Diagnostics;

namespace Core
{
    public static class PowershellExecutor
    {
        public static void RunPowershellInstaller(ProgramData programData)
        {
            // Create a process to execute PowerShell
            Process process = new();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            PowershellScriptBuilder.BuildPowershellInstallScript(process, programData);

            // Start the process
            process.Start();

            // Wait for the process to finish
            process.WaitForExit();
        }
    }
}
