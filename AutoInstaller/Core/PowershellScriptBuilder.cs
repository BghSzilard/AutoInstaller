using AISL;
using System.Diagnostics;

namespace Core
{
    public static class PowershellScriptBuilder
    {
        public static void BuildPowershellInstallScript(Process process, ProgramData programData)
        {
            process.StartInfo.FileName = "msiexec";
            process.StartInfo.Arguments = $" /i \"{programData.InstallerPath}\" ";
            if (programData.ParameterList != null)
            {
                foreach (var parameter in programData.ParameterList)
                {
                    process.StartInfo.Arguments += $"{parameter.Name}=\"{parameter.Value}\" ";
                }
            }
            process.StartInfo.Arguments += "/qb+";
        }
    }
}