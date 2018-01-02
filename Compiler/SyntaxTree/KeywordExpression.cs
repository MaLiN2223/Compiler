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
  }
}