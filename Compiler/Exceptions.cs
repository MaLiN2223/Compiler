using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Lexing;

namespace Compiler
{
	public class CompilationSyntaxErrorException : SyntaxErrorException
	{
		public CompilationSyntaxErrorException():base() { }
		public CompilationSyntaxErrorException(Token token) : base($"Error in {token.Value} line: {token.Line}") { }
	}
}
