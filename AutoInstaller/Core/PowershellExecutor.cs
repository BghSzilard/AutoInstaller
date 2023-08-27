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
        string script = ConvertToPowerShell();

        // Create a process to execute PowerShell
        Process process = new Process();
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        // Start the process
        process.Start();

        // Wait for the process to finish
        process.WaitForExit();

    }
    public string ConvertToPowerShell()
    {
        string powerShellCode = $"Start-Process {_scriptInfoExtractor.GetProgramInfo().InstallationsPath}";
        return powerShellCode;
    }
}