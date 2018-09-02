using Compiler.Language;

namespace Compiler.SyntaxTree
{
  public abstract class DataTypeExpression : AbstractExpression
  {
    public abstract ValueType ValueType { get; }
  }


  public abstract class DataTypeExpression<T> : DataTypeExpression
  {
    public DataTypeExpression(T value)
    {
      Value = value;
    }

    public T Value { get; set; }
  }
}