namespace Compiler.SyntaxTree
{
	public abstract class LogicalFunctionExpression : KeywordExpression, IScopedExpression
	{
		public LogicalFunctionExpression(string value, AbstractExpression logicalCheck, ScopeExpression bodyExpression)
			: base(value)
		{
			LogicalCheck = logicalCheck;
			BodyExpression = bodyExpression;
		}

		public AbstractExpression LogicalCheck { get; set; }
		public ScopeExpression BodyExpression { get; set; }
	}

	public class NamespaceExpression : ScopeExpression
	{
		public string NamespaceName { get; }

		public NamespaceExpression(string namespaceName, ScopeExpression scopeExpression)
		{
			NamespaceName = namespaceName;
			Expressions = scopeExpression.Expressions;
		}
	}

	public class ClassExpression : ScopeExpression
	{
		public string ClassName { get; }

		public ClassExpression(string className, ScopeExpression bodyExpression)
		{
			ClassName = className;
			Expressions = bodyExpression.Expressions;
		}
	}

	public class ElseExpression : KeywordExpression
	{
		public ScopeExpression BodyExpression { get; }

		public ElseExpression(string value, ScopeExpression bodyExpression)
			: base(value)
		{
			BodyExpression = bodyExpression;
		}
	}

	public class IfExpression : LogicalFunctionExpression
	{
		public IfExpression(string value, AbstractExpression logicalCheck, ScopeExpression bodyExpression)
			: base(value, logicalCheck, bodyExpression)
		{
		}
	}

	public class ReturnExpression : KeywordExpression
	{
		public AbstractExpression Expression { get; set; }
		public ReturnExpression(AbstractExpression expression) : base("return")
		{
			Expression = expression;
		}
	}
	public abstract class LoopExpression : LogicalFunctionExpression
	{
		public LoopExpression(string value, ScopeExpression logicalCheck, ScopeExpression bodyExpression)
			: base(value, logicalCheck, bodyExpression)
		{
		}
	}

	public class WhileLoopExpression : LogicalFunctionExpression
	{
		public WhileLoopExpression(AbstractExpression logicalCheck, ScopeExpression scopeExpression)
			: base("while", logicalCheck, scopeExpression)
		{
		}
	}

	public sealed class ForLoopExpression : LogicalFunctionExpression
	{
		public AbstractExpression Initialization { get; }

		public AbstractExpression Incrementation { get; }

		public ForLoopExpression(
			AbstractExpression logicalCheck,
			AbstractExpression initialization,
			AbstractExpression incrementation,
			ScopeExpression bodyExpression
		)
			: base("for", logicalCheck, bodyExpression)
		{
			Initialization = initialization;
			Incrementation = incrementation;
		}
	}
}
