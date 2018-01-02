using System.Collections.Generic;

namespace Compiler.SyntaxTree
{
  public class ScopeBegginingExpression : AbstractExpression
  {

  }
  public class ScopeExpression : AbstractExpression
  {
    public List<AbstractExpression> Expressions = new List<AbstractExpression>();

    public void Push(AbstractExpression expr)
    {
      Expressions.Add(expr);
    }
  }
}