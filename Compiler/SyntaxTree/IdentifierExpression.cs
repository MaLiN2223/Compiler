namespace Compiler.SyntaxTree
{
  public class IdentifierExpression : AbstractExpression
  {
    public IdentifierExpression(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}