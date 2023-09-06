using AISL;
using Antlr4.Runtime;
using System.Text;

namespace Core
{
    public static class ScriptDataExtractor
    {
        public static ProgramData GetProgramData(string pathToScript)
        {
	        string scriptText = File.ReadAllText(pathToScript, Encoding.UTF8).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            //string scriptText = """
            //    FIND "TCB" AT "C:/Users/Botond/Documents/Siemens/Summer School/Final/tcb (1)";
            //    HAS (
            //        string APPDIR WITH DEFAULT "C:\",
            //    ) AS installation_parameters;
            //    UNINSTALL "TCB";
            //    EXECUTE "Simcenter Test Cloud Blueprint Setup.exe" WITH installation_parameters;
                
            //    """;

            AntlrInputStream inputStream = new(scriptText);
            AISLLexer aislLexer = new(inputStream);
            CommonTokenStream commonTokenStream = new(aislLexer);
            AISLParser aislParser = new(commonTokenStream);

            AISLParser.ScriptContext scriptContext = aislParser.script();
            AISLScriptVisitor visitor = new();

            var programData = visitor.Visit(scriptContext);

            return programData;
        }
    }
}
