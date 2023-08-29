using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;


namespace AISL
{
	// in development
	public class SyntaxErrorException : Exception
	{
		public SyntaxErrorException(InputMismatchException exception)
		{
			Message = exception.Message;
		}

		public SyntaxErrorException(RecognitionException exception)
		{
			Message = exception.Message;
		}

		// maybe add more properties of InputMismatchException

		public override string Message { get; }
	}
}
