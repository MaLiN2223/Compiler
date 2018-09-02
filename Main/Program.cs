using System;
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
			bool dumpTree = true;
			var model = new LanguageModel();
			var lexer = new Lexer(model);
			var tokens = lexer.ParseFile(@"source.mal").ToList();
			var q = new AbstractSyntaxTreeBuilder(model);
			try
			{
				var e = new Emitter();
				var t = q.Build(tokens);
				if (dumpTree)
				{
					Console.WriteLine(t.Dump());
				}
				e.Emit(t, "Test");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadKey();
		}
	}
}
