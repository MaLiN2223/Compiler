using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Compiler.Emitting;
using Compiler.Language;
using Compiler.Lexing;
using Compiler.SyntaxTree;
using Xunit;

namespace Compiler.FunctionalTests
{
	public class DeclarationTests
	{
		private static string BasePath = Path.Combine(Path.GetTempPath(), "CompilerTesting");

		public DeclarationTests()
		{
			Directory.CreateDirectory(BasePath);
		}
		private string SaveToTempFile(string data)
		{
			var file = Path.Combine(BasePath, Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".mal");
			File.WriteAllText(file, data);
			return file;
		}

		private string Foo(string code)
		{
			var file = SaveToTempFile(code);

			var model = new LanguageModel();
			var lexer = new Lexer(model);
			var tokens = lexer.ParseFile(file).ToList();
			var q = new AbstractSyntaxTreeBuilder(model);
			var e = new Emitter();
			var t = q.Build(tokens);
			return e.Emit(t, file);
		}

		[Fact]
		public void ContainsMainMethod()
		{
			var code = "";
			var q = Foo(code);

			var assembly = Assembly.LoadFile(q);
			var type = assembly.GetType("Namespace.Program");
			var method = type.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);

			Assert.NotNull(method);
			Assert.Equal(typeof(void), method.ReturnType);
		}
	}
}
