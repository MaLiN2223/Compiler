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
      if (left is VariableExpression && value == "=")
      {
        return new AssignmentExpression(left as VariableExpression, right);
      }

      if (left is VariableExpression && right is VariableExpression)
      {
        return new OperatorExpression(value, left as VariableExpression, right as VariableExpression);
      }

      if (left is VariableExpression)
      {
        return new OperatorExpression(value, left as VariableExpression, right);
      }

      if (right is VariableExpression)
      {
        return new OperatorExpression(value, left, right as VariableExpression);
      }

      return new OperatorExpression(value, left, right);
    }
  }
}