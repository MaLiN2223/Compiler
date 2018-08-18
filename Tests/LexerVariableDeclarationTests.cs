using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Language;
using Compiler.Lexing;
using NSubstitute;
using Xunit;

namespace Tests
{
	public class LexerVariableDeclarationTests
	{
		private ILanguageModel model;
		private Lexer lexer;
		public LexerVariableDeclarationTests()
		{
			model = Substitute.For<ILanguageModel>();
			lexer = new Lexer(model);
			model.IsCommentCharacter(Arg.Any<char>()).Returns(false);
			model.IsCommentCharacter('#').Returns(true);
			model.IsEof(Arg.Any<char>()).Returns(false);
			model.IsEof('\0').Returns(true);
			model.IsDigit(Arg.Any<char>()).ReturnsForAnyArgs(x => char.IsDigit(x.Arg<char>()));
			model.IsBeginingOfIdentifier(Arg.Any<char>()).ReturnsForAnyArgs(x => char.IsLetter(x.Arg<char>()));
			model.IsPartOfDigit(Arg.Any<char>()).ReturnsForAnyArgs(x => char.IsDigit(x.Arg<char>()));
			model.IsMiddleIdentifier(Arg.Any<char>()).ReturnsForAnyArgs(x => char.IsLetter(x.Arg<char>()));
		}	
		[Fact]
		public void OneCharVariableNameDeclaration()
		{
			string code = "int i;\0";
			var tokens = lexer.ParseString(code).ToList();
			Assert.Equal(3, tokens.Count);
			Assert.Equal(TokenType.DataType, tokens[0].Type);
			Assert.Equal(TokenType.Identifier, tokens[1].Type);
			Assert.Equal(TokenType.Terminator, tokens[2].Type);

		}
	}
}
