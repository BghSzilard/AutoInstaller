using Antlr4.Runtime;
using System.Text;

namespace AISL;

class Program
{
    private static void Main(string[] args)
    {
        try
        {
            string scriptText = File.ReadAllText(@"D:\Siemens\AutoInstaller\AutoInstaller\AISL\script.aisl", Encoding.UTF8);

            AntlrInputStream inputStream = new AntlrInputStream(scriptText);
            AISLLexer aislLexer = new AISLLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(aislLexer);
            AISLParser aislParser = new AISLParser(commonTokenStream);

            AISLParser.ScriptContext scriptContext = aislParser.script();
            AISLScriptVisitor visitor = new();
            var programInfo = visitor.Visit(scriptContext);

            Console.WriteLine();

            // using PowerShell ps = PowerShell.Create();
            // ps.AddScript($"Start-Process {visitor.Tokens[1]}");
            // ps.Invoke();

            //foreach (var token in visitor.Tokens)
            //{
            //    Console.WriteLine(token);
            //}
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }
}