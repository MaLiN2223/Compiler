using System.Collections.Generic;

namespace Compiler.SyntaxTree
{
	public class ScopeBegginingExpression : AbstractExpression
	{

	}
	public class ScopeExpression : AbstractExpression
	{
		public List<AbstractExpression> Expressions = new List<AbstractExpression>();
		public string TypeName { get; private set; }

		public void Push(AbstractExpression expr, string typeName)
		{
			TypeName = typeName;
			Expressions.Add(expr);
		}
	}
}