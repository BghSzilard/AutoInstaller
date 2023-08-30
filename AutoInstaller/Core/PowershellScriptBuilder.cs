using AISL;

namespace Core
{
    public static class PowershellScriptBuilder
    {
        public static string BuildPowershellInstallScript(ProgramData programData)
        {
            string script = "";
            script += programData.InstallerPath;
            script += " /qb+";
            return script;
        }
    }
}
