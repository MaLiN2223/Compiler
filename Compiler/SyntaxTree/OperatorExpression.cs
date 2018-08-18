namespace Compiler.SyntaxTree
{
  public class OperatorExpression : AbstractExpression
  {
    public OperatorExpression(string operation, AbstractExpression left, AbstractExpression right)
    {
      Left = left;
      Right = right;
      Operation = operation;
    }

    public AbstractExpression Left { get; }

    public string Operation { get; }

    public AbstractExpression Right { get; }

    public static OperatorExpression Operator(string value, AbstractExpression left, AbstractExpression right)
    {
      if (left is IdentifierExpression && value == "=")
      {
        return new AssignmentExpression(left as IdentifierExpression, right);
      }

      if (left is IdentifierExpression && right is IdentifierExpression)
      {
        return new OperatorExpression(value, left as IdentifierExpression, right as IdentifierExpression);
      }

      if (left is IdentifierExpression)
      {
        return new OperatorExpression(value, left as IdentifierExpression, right);
      }

      if (right is IdentifierExpression)
      {
        return new OperatorExpression(value, left, right as IdentifierExpression);
      }

      return new OperatorExpression(value, left, right);
    }
  }
}