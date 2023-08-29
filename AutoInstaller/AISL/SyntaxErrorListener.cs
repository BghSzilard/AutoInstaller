namespace AISL;

using Antlr4.Runtime;
using System.IO;

public class SyntaxErrorListener : BaseErrorListener
{
	public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
		string msg, RecognitionException e)
	{
		//base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
		throw new Exception($"{line}:{charPositionInLine}: {msg}");
	}
}
