using Compiler.Language;

namespace Compiler.SyntaxTree.DataTypeExpressions
{
  public class Int32Expression : DataTypeExpression<int>
  {
    public Int32Expression(int value)
      : base(value)
    {
    }

    public override ValueType ValueType => ValueType.Int;
  }
}