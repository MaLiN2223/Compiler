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
      var a = string.Join(",", tokens.Select(x => x.Value));
      var postfix = PostfixCreator.Postfix(tokens);
      var q = string.Join(",", postfix.Select(x => x.Value));
      var mainBlock = ConsumeNewScope(postfix.GetEnumerator());
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

    private ScopeExpression ConsumeNewScope(IEnumerator<Token> instruction)
    {
      var stack = new Stack<AbstractExpression>();
      var scope = new ScopeExpression();
      var instructionsStack = new Stack<AbstractExpression>();
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
            instructionsStack.Push(stack.Pop());
          }
        }
        else if (type == TokenType.Keyword)
        {
          instructionsStack.Push(ConsumeKeyword(token.Value, stack, instructionsStack, enumer));
        }
        else if (type == TokenType.Scope)
        {
          if (token.Value == "{")
          {
            var innerScope = ConsumeNewScope(enumer);
            instructionsStack.Push(innerScope);
          }
          else if (token.Value == "}")
          {
            while (stack.Count > 0)
            {
              throw new ArgumentException();
            }
            scope.Expressions = instructionsStack.Reverse().ToList();
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
        throw new ArgumentException();
      }
      scope.Expressions = instructionsStack.Reverse().ToList();
      return scope;
    }
    private KeywordExpression ConsumeKeyword(
      string value, Stack<AbstractExpression> stack, Stack<AbstractExpression> scopeStack, IEnumerator<Token> enumer)
    {
      KeywordExpression expr = null;
      enumer.MoveNext(); // skip current keyword
      var innerScope = ConsumeNewScope(enumer);

      if (value == "if")
      {
        var logicalCondition = stack.Pop();
        expr = new LogicalFunctionExpression(value, logicalCondition);
      }
      else if (value == "while")
      {
        var logicalCondition = stack.Pop();
        expr = new WhileLoop(value, logicalCondition);
      }
      else if (value == "for")
      {
        var incr = stack.Pop();
        var condition = scopeStack.Pop();
        var assignment = scopeStack.Pop();
        expr = new ForLoop(value, condition, assignment, incr);
      }
      else
      {
        expr = new KeywordExpression(value);
      }
      expr.ScopeExpression = innerScope;
      return expr;
    }
  }
}