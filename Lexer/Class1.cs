using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
  public enum TokenType
  {
    Identifier, // 
    Operator,
    Keyword,
    Punctuation, // Comma, dot, bracket etc
    Comment,
    Number

  }

  public struct Token
  {
    public Token(string value, TokenType type)
    {
      Value = value;
      Type = type;
    }
    public TokenType Type { get; set; }
    public string Value { get; set; }


  }


  public static class Specials
  {
    public static char Comment = '#';
  }
  public class Lexer
  {
    private readonly string _file;

    public string[] Keywords = { "if", "else", "true", "false" };

    public char[] Punctuations = { '.', ',', ')', '(' };

    private char[] operators = { '+', '-', '/', '*', '%', '!', '<', '>' };


    public Lexer(string file)
    {
      _file = file;
    }

    public IEnumerable<Token> DoParse(string filePath)
    {
      var enumerator = SourceEnumerator(filePath);
      bool first = false;
      while (first || enumerator.MoveNext())
      {
        first = true;
        var current = enumerator.Current;
        if (char.IsDigit(current))
        {
          yield return ReadNumber(enumerator);
        }
        if (IsPunctuation(current))
        {
          yield return ReadPunctuation(enumerator);
        }
        if (IsOperator(current))
        {
          yield return ReadOperator(enumerator);
        }
        if (IsBeginingOfIdentifier(current))
        {
          yield return ReadIdentifier(enumerator);
        }
        while (IsEmpty(current) && enumerator.MoveNext())
        {
          // Ignore
        }
      }
    }

    #region Logic predicates
    private bool IsBeginingOfIdentifier(char current)
    {
      return char.IsLetter(current) || current == '_';
    }

    private bool IsEmpty(char current)
    {
      return current == ' ' || current == '\n' || current == '\t';
    }

    private bool IsMiddleIdentifier(char current)
    {
      return IsBeginingOfIdentifier(current) || char.IsNumber(current);
    }

    private bool IsOperator(char current)
    {
      return operators.Contains(current);
    }
    private bool IsPunctuation(char current)
    {
      return Punctuations.Contains(current);
    }

    #endregion

    #region Readers
    private Token ReadNumber(IEnumerator<char> enumerator)
    {
      var number = ReadWhilePredicate(enumerator, char.IsDigit);
      return new Token(number, TokenType.Number);
    }
    private Token ReadOperator(IEnumerator<char> enumerator)
    {
      string op = ReadWhilePredicate(enumerator, IsOperator);
      return new Token(op, TokenType.Operator);
    }
    private Token ReadPunctuation(IEnumerator<char> enumerator)
    {
      string punc = ReadWhilePredicate(enumerator, IsPunctuation);
      return new Token(punc, TokenType.Punctuation);
    }

    private Token ReadIdentifier(IEnumerator<char> enumerator)
    {
      string ident = ReadWhilePredicate(enumerator, IsMiddleIdentifier);
      if (Keywords.Contains(ident))
      {
        return new Token(ident, TokenType.Keyword);
      }
      else
      {
        return new Token(ident, TokenType.Identifier);
      }

    }
    #endregion





    private string ReadWhilePredicate(IEnumerator<char> enumerator, Func<char, bool> predicate)
    {
      string x = "";
      do
      {
        x += enumerator.Current;
      } while (enumerator.MoveNext() && predicate(enumerator.Current));
      return x;
    }
    private IEnumerator<char> SourceEnumerator(string file)
    {
      var f = File.ReadAllLines(file);
      foreach (var line in f)
      {
        var lineEnumerator = line.GetEnumerator();
        foreach (var ch in Iterate(lineEnumerator))
        {
          yield return ch;
        }
      }
    }
    private IEnumerable<char> Iterate(CharEnumerator source)
    {
      while (source.MoveNext())
      {
        char ch = source.Current;
        //if (ch == ' ')
        //{
        //  continue;
        //}
        if (ch == Specials.Comment)
        {
          yield break;
        }
      }
    }


  }
}
