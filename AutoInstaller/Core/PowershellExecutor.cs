using AISL;
using System.Diagnostics;
using System.Management.Automation;

namespace Core;

public class PowershellExecutor
{
    private ScriptInfoExtractor _scriptInfoExtractor { get; set; }
    public PowershellExecutor(ScriptInfoExtractor scriptInfoExtractor)
    {
        _scriptInfoExtractor = scriptInfoExtractor;
    }
    public void RunPowershellInstaller()
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
    public void AddPath(Process process)
    {
        string pathToAdd = _scriptInfoExtractor.GetProgramInfo().InstallationsPath;
        process.StartInfo.Arguments += $"\"{pathToAdd}\"";
    }
}