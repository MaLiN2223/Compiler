using Compiler.Language;

namespace Compiler.SyntaxTree.DataTypeExpressions
{
  public class DoubleExpression : DataTypeExpression<double>
  {
    public DoubleExpression(double value)
      : base(value)
    {
    }

    public override ValueType ValueType { get; }
  }
}