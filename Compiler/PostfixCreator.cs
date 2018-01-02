using System;
using System.Collections.Generic;
using System.Linq;

using Compiler.Language;
using Compiler.Lexing;

namespace Compiler
{
  public static class PostfixCreator
  {
    public static List<Token> Postfix(IEnumerable<Token> token)
    {
      return InnerPostfix(token).ToList();
    }

    private static IEnumerable<Token> InnerPostfix(IEnumerable<Token> tokens)
    {
      var stack = new Stack<Token>();
      var enu = tokens.GetEnumerator();

      var output = new Queue<Token>();
      while (enu.MoveNext())
      {
        var token = enu.Current;
        var type = token.Type;
        if (type == TokenType.DataType || type == TokenType.Identifier)
        {
          output.Enqueue(token);
        }
        else if (type == TokenType.Keyword)
        {
          stack.Push(token);
        }
        else if (token.Value == "=")
        {
          stack.Push(token);
        }
        else if (type == TokenType.Operator)
        {
          var data = EvaluationData.OperatorData[token.Value];
          while (stack.Count > 0 && stack.Peek().Type == TokenType.Operator)
          {
            var tmp = stack.Peek();
            var tmpData = EvaluationData.OperatorData[tmp.Value];
            if (data.Associativity == Associativity.Left && data.Priority <= tmpData.Priority
                || data.Associativity == Associativity.Right && data.Priority < tmpData.Priority)
            {
              tmp = stack.Pop();
              output.Enqueue(tmp);
            }
            else
            {
              break;
            }
          }

          stack.Push(token);
        }
        else if (type == TokenType.Punctuation)
        {
          if (token.Value == "(")
          {
            stack.Push(token);
          }
          else if (token.Value == ")")
          {
            var q = stack.Pop();
            while (q.Value != "(")
            {
              output.Enqueue(q);
              q = stack.Pop();
            }

            if (stack.Count > 0 && LanguageModel.IsKeywordWithScope(stack.Peek().Value))
            {
              output.Enqueue(stack.Pop());
            }
          }
        }
        else if (token.Value == "{")
        {
          while (output.Count > 0)
          {
            yield return output.Dequeue();
          }
          while (stack.Count > 0)
          {
            yield return stack.Pop();
          }

          yield return token;
        }
        else if (type == TokenType.Terminator)
        {
          Token? lastTaken = null;
          while (
            stack.Count > 0
            && stack.Peek().Value != "{"
            && stack.Peek().Value != "(")
          {
            lastTaken = stack.Pop();
            output.Enqueue(lastTaken.Value);
          }

          foreach (var t in output)
          {
            yield return t;
          }

          output.Clear();
          yield return token;
        }
        else if (token.Value == "}")
        {
          if (stack.Count > 0 || output.Count > 0)
          {
            throw new ArgumentException("Something went wrong, probably missing ;");
          }

          yield return token;
        }
        else if (type == TokenType.Type)
        {
          // TODO : fix this hack
          enu.MoveNext();
          output.Enqueue(enu.Current);
          output.Enqueue(token);
        }
        else
        {
          throw new ArgumentException($"Unknown token {token.Value}");
        }
      }

      if (stack.Count > 0 || output.Count > 0)
      {
        throw new ArgumentException("Something went wrong, probably missing ;");
      }
    }
  }
}