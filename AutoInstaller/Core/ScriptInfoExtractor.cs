using AISL;
using Antlr4.Runtime;
using System.Text;

namespace Core
{
    public static class ScriptInfoExtractor
    {
        public static ProgramData GetVersionInfo(string programName, string version)
        {
            string targetFile = Path.Combine(ProgramService._programsPath, programName, version + ".aisl");
            
            string scriptText = File.ReadAllText(targetFile, Encoding.UTF8);

            AntlrInputStream inputStream = new AntlrInputStream(scriptText);
            AISLLexer aislLexer = new AISLLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
            AISLParser aislParser = new AISLParser(commonTokenStream);

            AISLParser.ScriptContext scriptContext = aislParser.script();
            AISLScriptVisitor visitor = new();
            var programInfo = visitor.Visit(scriptContext);
            return programInfo;
        }
        public static ProgramData GetProgramInfo(string programName)
        {
            string targetFile = Path.Combine(ProgramService._programsPath, programName);
            List<string> files = ProgramService.FindFiles(targetFile);

            targetFile = Path.Combine(targetFile, files[0]);
            string scriptText = File.ReadAllText(targetFile, Encoding.UTF8);

            AntlrInputStream inputStream = new AntlrInputStream(scriptText);
            AISLLexer aislLexer = new AISLLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
            AISLParser aislParser = new AISLParser(commonTokenStream);

            AISLParser.ScriptContext scriptContext = aislParser.script();
            AISLScriptVisitor visitor = new();
            var programInfo = visitor.Visit(scriptContext);
            return programInfo;
        }

        public static ProgramData GetScriptInfo(string pathToScript)
        {
            string scriptText = File.ReadAllText(pathToScript, Encoding.UTF8);

            AntlrInputStream inputStream = new AntlrInputStream(scriptText);
            AISLLexer aislLexer = new AISLLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
            AISLParser aislParser = new AISLParser(commonTokenStream);

            AISLParser.ScriptContext scriptContext = aislParser.script();
            AISLScriptVisitor visitor = new();
            var programInfo = visitor.Visit(scriptContext);
            return programInfo;
        }
    }
}
