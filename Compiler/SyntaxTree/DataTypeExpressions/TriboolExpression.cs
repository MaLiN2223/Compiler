using ValueType = Compiler.Language.ValueType;

namespace Compiler.SyntaxTree.DataTypeExpressions
{
  public class TriboolExpression : DataTypeExpression<bool?>
  {
    public TriboolExpression(bool? value)
      : base(value)
    {
    }

    public override ValueType ValueType { get; }
  }
}
