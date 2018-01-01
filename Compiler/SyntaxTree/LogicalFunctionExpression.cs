namespace Compiler.SyntaxTree
{
  public class LogicalFunctionExpression : KeywordExpression
  {
    public LogicalFunctionExpression(string value, AbstractExpression innerExpression)
      : base(value)
    {
      InnerExpression = innerExpression;
    }

    public AbstractExpression InnerExpression { get; set; }
  }
}