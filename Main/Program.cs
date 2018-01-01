using System.Linq;

using Compiler;
using Compiler.Emitting;
using Compiler.Lexing;
using Compiler.SyntaxTree;

namespace Main
{
  class Program
  {
    static void Main(string[] args)
    {
      var lexer = new Lexer();
      var tokens = lexer.DoParse(@"source.mal").ToList();
      var q = new AbstractSyntaxTreeBuilder();
      var e = new Emitter();
      var t = q.Build(tokens);
      e.Emit(t);
    }
  }
}
