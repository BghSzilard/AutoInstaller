using Antlr4.Runtime;
using System.Text;
using static AISLParser;

namespace AISL;

public class ScriptInfoExtractor
{
    public ProgramInfo GetProgramInfo()
    {
        try
        {
            string targetFile = Enumerable.Range(0, 4)
            .Aggregate(Environment.CurrentDirectory, (current, _) =>
                Path.GetDirectoryName(current) ?? throw new Exception("Cannot find the folder containing the script"));
            targetFile = Path.Combine(targetFile, "AISL", "script.aisl");
            string scriptText = File.ReadAllText(targetFile, Encoding.UTF8);

            CustomErrorListener errorListener = new CustomErrorListener();

            AntlrInputStream inputStream = new AntlrInputStream(scriptText);
            AISLLexer aislLexer = new AISLLexer(inputStream);
            aislLexer.AddErrorListener(errorListener);

            CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
            AISLParser aislParser = new AISLParser(commonTokenStream);
            //aislParser.AddErrorListener(errorListener);

            AISLParser.ScriptContext scriptContext = aislParser.script();
            AISLScriptVisitor visitor = new();
            var programInfo = visitor.Visit(scriptContext);
            return programInfo;    
        }
        catch(RecognitionException e)
        {
            Console.WriteLine(e.Message);
            return new ProgramInfo(); // temporary solution
        }
    }
}