using System;
using System.Collections.Generic;

using Compiler.Language;

namespace Compiler.Lexing
{
  public class Lexer
  {
    public IEnumerable<Token> DoParse(string file)
    {
      var enumerator = new FileEnumerator(file).GetEnumerator();
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
          yield return new Token(";", TokenType.Terminator);
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