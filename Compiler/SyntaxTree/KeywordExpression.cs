namespace Compiler.SyntaxTree
{
  public class KeywordExpression : AbstractExpression
  {
    public KeywordExpression(string value)
    {
      Value = value;
    }

    public ScopeExpression ScopeExpression { get; set; }

    public string Value { get; }

    public static KeywordExpression Keyword(string value, AbstractExpression expression, ScopeExpression scope)
    {
      KeywordExpression expr = null;
      if (value == "if" || value == "while")
      {
        expr = new LogicalFunctionExpression(value, expression);
      }
      else
      {
        expr = new KeywordExpression(value);
      }

      expr.ScopeExpression = scope;
      return expr;
    }
  }
}