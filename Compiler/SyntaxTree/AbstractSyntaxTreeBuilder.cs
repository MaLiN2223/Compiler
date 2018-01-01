using System;
using System.Collections.Generic;
using System.Linq;

using Compiler.Language;
using Compiler.Lexing;
using Compiler.SyntaxTree.DataTypeExpressions;

namespace Compiler.SyntaxTree
{
  public class AbstractSyntaxTreeBuilder
  {
    public ScopeExpression Build(IEnumerable<Token> tokens)
    {
      var postfix = PostfixCreator.Postfix(tokens);
      var q = string.Join(",", postfix.Select(x => x.Value));
      var mainBlock = HandleInstruction(postfix.GetEnumerator());
      return mainBlock;
    }

    private AbstractExpression CreateNumberExpression(string value)
    {
      if (LanguageModel.IsDataTypeKeyword(value))
      {
        // For now we only suppor true/false
        return new LogicExpression(value);
      }

      if (int.TryParse(value, out var q))
      {
        return new Int32Expression(q);
      }

      if (double.TryParse(value, out var z))
      {
        return new DoubleExpression(z);
      }

      throw new NotSupportedException();
    }

    private ScopeExpression HandleInstruction(IEnumerator<Token> instruction)
    {
      var stack = new Stack<AbstractExpression>();
      var scope = new ScopeExpression();
      var enumer = instruction;
      while (enumer.MoveNext())
      {
        var token = enumer.Current;
        var type = token.Type;
        if (type == TokenType.DataType)
        {
          stack.Push(CreateNumberExpression(token.Value));
        }
        else if (type == TokenType.Identifier)
        {
          stack.Push(new VariableExpression(token.Value));
        }
        else if (type == TokenType.Type)
        {
          if (stack.Pop() is VariableExpression variable)
          {
            stack.Push(new DeclarationExpression(token.Value, variable.Name));
          }
          else
          {
            throw new ArgumentException();
          }
        }
        else if (type == TokenType.Terminator)
        {
          while (stack.Count > 0 && !(stack.Peek() is ScopeExpression))
          {
            scope.Push(stack.Pop());
          }
        }
        else if (type == TokenType.Keyword)
        {
          var br = enumer.MoveNext();
          var sc = HandleInstruction(enumer);
          scope.Push(KeywordExpression.Keyword(token.Value, stack.Pop(), sc));
        }
        else if (type == TokenType.Scope)
        {
          if (token.Value == "{")
          {
            var innerScope = HandleInstruction(enumer);
            scope.Push(innerScope);
          }
          else if (token.Value == "}")
          {
            while (stack.Count > 0)
            {
              scope.Push(stack.Pop());
            }

            return scope;
          }
        }
        else if (type == TokenType.Operator)
        {
          var a = stack.Pop();
          var b = stack.Pop();
          stack.Push(OperatorExpression.Operator(token.Value, b, a));
        }
      }

      while (stack.Count > 0)
      {
        scope.Push(stack.Pop());
      }

      return scope;
    }
  }
}