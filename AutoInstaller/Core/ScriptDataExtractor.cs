using AISL;
using Antlr4.Runtime;
using System.Text;

namespace Core
{
    public static class ScriptDataExtractor
    {
        public static ProgramData GetProgramData(string pathToScript)
        {
            string scriptText = File.ReadAllText(pathToScript, Encoding.UTF8);

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
