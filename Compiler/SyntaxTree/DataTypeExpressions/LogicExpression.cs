using System;

using ValueType = Compiler.Language.ValueType;

namespace Compiler.SyntaxTree.DataTypeExpressions
{
  public class LogicExpression : DataTypeExpression<bool>
  {
    public LogicExpression(string value)
      : base(ToBool(value))
    {
    }

    public override ValueType ValueType => ValueType.Bool;

    private static bool ToBool(string value)
    {
      if (value == "true")
      {
        return true;
      }

      if (value == "false")
      {
        return false;
      }

      throw new ArgumentException();
    }
  }
}