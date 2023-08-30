using AISL;
using System.Diagnostics;

namespace Core
{
    public static class PowershellExecutor
    {
        //public static void RunPowershellInstallCommand(ProgramData programData)
        //{
        //    //string script = PowershellScriptBuilder.BuildPowershellInstallScript(programData);
        //    string script = "";
        //    // Create a process to execute PowerShell

        //    Process process = new Process();
        //    process.StartInfo.FileName = "powershell.exe";
        //    process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"";
        //    process.StartInfo.FileName = "msiexec";
        //    process.StartInfo.Arguments = "/i C:\\Users\\sziba\\Desktop\\BraveBrowserSetup-BRV010.exe /qb+";
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardError = true;
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;

        //    //AddPath(process);

        //    //process.StartInfo.Arguments += "/qb+";

        //    // Start the process
        //    process.Start();

        //    // Wait for the process to finish
        //    process.WaitForExit();
        //}
        public static void RunPowershellInstaller()
        {
            
            // Create a process to execute PowerShell
            Process process = new Process();
            process.StartInfo.FileName = "msiexec";
            process.StartInfo.Arguments = "/i";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            AddPath(process);

            process.StartInfo.Arguments += "/qb+";

            // Start the process
            process.Start();

            // Wait for the process to finish
            process.WaitForExit();

        }
        public static void AddPath(Process process)
        {
            string powerShellCode = $"D:\\Summer School 2023\\asd\\asd\\230822_1.1.9_core\\Simcenter Test Cloud Blueprint Setup.msi /qb+";
            process.StartInfo.Arguments += powerShellCode;
        }

    }
}
