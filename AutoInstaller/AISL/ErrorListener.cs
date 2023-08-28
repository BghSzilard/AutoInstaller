namespace AISL;

using Antlr4.Runtime;
using System.IO;

public class CustomErrorListener : IAntlrErrorListener<int>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new NotImplementedException();
    }
}
