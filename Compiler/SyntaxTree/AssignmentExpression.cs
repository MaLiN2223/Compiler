using System;

namespace Compiler.SyntaxTree
{
  public class AssignmentExpression : OperatorExpression
  {
    public AssignmentExpression(VariableExpression left, AbstractExpression right)
      : base("=", left, right)
    {
      Left = left ?? throw new ArgumentException("left side of expression cannot be null");
      Right = right ?? throw new ArgumentException("right side of expression cannot be null");
    }

    public VariableExpression Left { get; }

    public AbstractExpression Right { get; }
  }
}