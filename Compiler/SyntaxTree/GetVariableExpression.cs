namespace Compiler.SyntaxTree
{
  public class GetVariableExpression : OperatorExpression
  {
    public GetVariableExpression(string value, AbstractExpression left, AbstractExpression right)
      : base(value, left, right)
    {
    }
  }
}