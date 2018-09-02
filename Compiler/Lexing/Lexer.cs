using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Compiler.Language;

namespace Compiler.Lexing
{
	public class Lexer
	{
		private readonly ILanguageModel languageModel;

		public Lexer(ILanguageModel languageModel)
		{
			this.languageModel = languageModel;
			CurrentLine = 1;
		}
		private class TextReader
		{
			private readonly string _text;
			private int _position;
			private int _textSize;
			public const char InvalidCharacter = char.MaxValue;


			public TextReader(string text)
			{
				_text = text;
				_position = 0;
				_textSize = text.Length;
			}

			public char PeekChar()
			{
				if (_position <= _textSize)
				{
					return _text[_position];
				}
				else
				{
					return InvalidCharacter;
				}
			}
			public void AdvanceChar()
			{
				_position++;
			}

			public char NextChar()
			{
				char c = PeekChar();
				if (c != InvalidCharacter)
				{
					AdvanceChar();
				}
				return c;
			}

		}

		private bool IsIdentifierPartCharacter(char c)
		{
			return SyntaxFacts.IsIdentifierPartCharacter(c);
		}

		private void ScanStringLiteral(TextReader text, Token token)
		{
			var builder = new StringBuilder();
			var quoteCharacter = text.PeekChar();

			while (true)
			{
				text.AdvanceChar();
				char ch = text.PeekChar();
				if (ch == quoteCharacter)
				{
					text.AdvanceChar(); // end of string literal
					break;
				}
				else
				{
					builder.Append(ch);
					text.AdvanceChar();
				}
			}

			if (quoteCharacter == '"')
			{
				token.SyntaxKind = SyntaxKind.StringLiteralExpression;
				if (builder.Length > 0)
				{
					token.Value = builder.ToString();
				}
				else
				{
					token.Value = string.Empty;
				}
			}
			else // TODO: handle chars
			{

			}


		}

		public IEnumerable<Token> ParseString(string data)
		{
			return Parse(data.GetEnumerator());
		}

		private int CurrentLine;
		private IEnumerable<Token> Parse(IEnumerator<char> enumerator)
		{
			enumerator.MoveNext();
			while (true)
			{
				var current = enumerator.Current;
				if (current == '\n')
				{
					CurrentLine++;
					enumerator.MoveNext();
				}
				if (languageModel.IsEof(current))
				{
					yield break;
				}

				if (languageModel.IsInstructionTerminator(current))
				{
					yield return new Token(";", TokenType.Terminator, CurrentLine, SyntaxKind.SemicolonToken);
					enumerator.MoveNext();
				}

				if (languageModel.IsDigit(current))
				{
					yield return ReadNumber(enumerator);
				}

				if (languageModel.IsPunctuation(current))
				{
					yield return ReadPunctuation(enumerator);
				}

				if (languageModel.IsOperator(current))
				{
					yield return ReadOperator(enumerator);
				}

				if (languageModel.IsBeginingOfIdentifier(current))
				{
					yield return ReadIdentifier(enumerator);
				}

				if (languageModel.IsScopeIndicator(current))
				{
					yield return new Token(current.ToString(), TokenType.Scope, CurrentLine);
					enumerator.MoveNext();
				}

				while (enumerator.Current == ' ' || enumerator.Current == '\t')
				{
					enumerator.MoveNext();
				}
			}
		}
		public IEnumerable<Token> ParseFile(string file)
		{
			return Parse(new FileEnumerator(file).GetEnumerator());
		}

		private Token ReadIdentifier(IEnumerator<char> enumerator)
		{
			var ident = ReadWhilePredicate(enumerator, languageModel.IsMiddleIdentifier);
			if (languageModel.IsSpecialDataKeyword(ident))
			{
				return new Token(ident, TokenType.SpecialDataKeyword, CurrentLine);
			}

			if (languageModel.IsKeyword(ident))
			{
				return new Token(ident, TokenType.Keyword, CurrentLine);
			}

			if (languageModel.IsType(ident))
			{
				return new Token(ident, TokenType.SimpleType, CurrentLine);
			}

			return new Token(ident, TokenType.Identifier, CurrentLine);
		}

		private Token ReadNumber(IEnumerator<char> enumerator)
		{
			var number = ReadWhilePredicate(enumerator, languageModel.IsPartOfDigit);
			return new Token(number, TokenType.SpecialDataKeyword, CurrentLine);
		}

		private Token ReadOperator(IEnumerator<char> enumerator)
		{
			var op = ReadWhilePredicate(enumerator, languageModel.IsOperator);
			return new Token(op, TokenType.Operator, CurrentLine);
		}

		private Token ReadPunctuation(IEnumerator<char> enumerator)
		{
			var punc = ReadWhilePredicate(enumerator, languageModel.IsPunctuation);
			return new Token(punc, TokenType.Punctuation, CurrentLine);
		}

		private string ReadWhilePredicate(IEnumerator<char> enumerator, Func<char, bool> predicate)
		{
			var x = string.Empty;
			do
			{
				x += enumerator.Current;
			}
			while (enumerator.MoveNext() && predicate(enumerator.Current));
			return x;
		}
	}
}