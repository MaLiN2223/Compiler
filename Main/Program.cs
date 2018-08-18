using System.Linq;

using Compiler;
using Compiler.Emitting;
using Compiler.Language;
using Compiler.Lexing;
using Compiler.SyntaxTree;

namespace Main
{
	class Program
	{
		static void Main(string[] args)
		{
			var model = new LanguageModel();
			var lexer = new Lexer(model);
			var tokens = lexer.ParseFile(@"source.mal").ToList();
			var q = new AbstractSyntaxTreeBuilder(model);
			var e = new Emitter();
			var t = q.Build(tokens);
			e.Emit(t, "Test");
		}
	}
}
