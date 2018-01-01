namespace Compiler.SyntaxTree
{
  public class VariableExpression : AbstractExpression
  {
    public VariableExpression(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}