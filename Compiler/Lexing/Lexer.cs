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
			return Parse(new StringEnumerator(data).GetEnumerator());
		}

		private IEnumerable<Token> Parse(IEnumerator<char> enumerator)
		{
			enumerator.MoveNext();
			while (true)
			{
				var current = enumerator.Current;
				if (LanguageModel.IsEof(current))
				{
					yield break;
				}

				if (LanguageModel.IsInstructionTerminator(current))
				{
					yield return new Token(";", TokenType.Terminator, SyntaxKind.SemicolonToken);
					enumerator.MoveNext();
				}

				if (LanguageModel.IsDigit(current))
				{
					yield return ReadNumber(enumerator);
				}

				if (LanguageModel.IsPunctuation(current))
				{
					yield return ReadPunctuation(enumerator);
				}

				if (LanguageModel.IsOperator(current))
				{
					yield return ReadOperator(enumerator);
				}

				if (LanguageModel.IsBeginingOfIdentifier(current))
				{
					yield return ReadIdentifier(enumerator);
				}

				if (LanguageModel.IsscopeIndicator(current))
				{
					yield return new Token(current.ToString(), TokenType.Scope);
					enumerator.MoveNext();
				}

				while (LanguageModel.IsEmpty(enumerator.Current))
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
			var ident = ReadWhilePredicate(enumerator, LanguageModel.IsMiddleIdentifier);
			if (LanguageModel.IsDataTypeKeyword(ident))
			{
				return new Token(ident, TokenType.DataType);
			}

			if (LanguageModel.IsKeyword(ident))
			{
				return new Token(ident, TokenType.Keyword);
			}

			if (LanguageModel.IsType(ident))
			{
				return new Token(ident, TokenType.Type);
			}

			return new Token(ident, TokenType.Identifier);
		}

		private Token ReadNumber(IEnumerator<char> enumerator)
		{
			var number = ReadWhilePredicate(enumerator, LanguageModel.IsPartOfDigit);
			return new Token(number, TokenType.DataType);
		}

		private Token ReadOperator(IEnumerator<char> enumerator)
		{
			var op = ReadWhilePredicate(enumerator, LanguageModel.IsOperator);
			return new Token(op, TokenType.Operator);
		}

		private Token ReadPunctuation(IEnumerator<char> enumerator)
		{
			var punc = ReadWhilePredicate(enumerator, LanguageModel.IsPunctuation);
			return new Token(punc, TokenType.Punctuation);
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