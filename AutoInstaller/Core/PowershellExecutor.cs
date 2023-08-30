using AISL;
using System.Diagnostics;

namespace Core
{
    public static class PowershellExecutor
    {
        public static void RunPowershellInstaller(ProgramData programData)
        {
            
            // Create a process to execute PowerShell
            Process process = new Process();
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
        public static void BuildScript(Process process, ProgramData programData)
        {
            //string powerShellCode = " \"D:\\Summer School 2023\\asd\\asd\\230822_1.1.9_core\\Simcenter Test Cloud Blueprint Setup.msi\"";

            //process.StartInfo.Arguments += powerShellCode;
        }
    }
}
