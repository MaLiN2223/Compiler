namespace Compiler.SyntaxTree
{
	public abstract class LogicalFunctionExpression : KeywordExpression
	{
		public LogicalFunctionExpression(string value, AbstractExpression logicalCheck)
			: base(value)
		{
			LogicalCheck = logicalCheck;
		}

		public AbstractExpression LogicalCheck { get; set; }
	}

	public class NamespaceExpression : KeywordExpression
	{
		public string NamespaceName { get; }

		public NamespaceExpression(string value, string namespaceName) : base(value)
		{
			NamespaceName = namespaceName;
		}
	}

	public class ClassExpression : KeywordExpression
	{
		public string ClassName { get; }
		public ClassExpression(string value, string className) : base(value)
		{
			ClassName = className;
		}
	}
	public class ElseExpression : KeywordExpression
	{
		public ElseExpression(string value)
			: base(value)
		{
		}
	}
	public class IfExpression : LogicalFunctionExpression
	{
		public IfExpression(string value, AbstractExpression logicalCheck)
			: base(value, logicalCheck)
		{
		}
	}

	public abstract class LoopExpression : LogicalFunctionExpression
	{
		public LoopExpression(string value, AbstractExpression logicalCheck)
			: base(value, logicalCheck)
		{
		}
	}

	public class WhileLoop : ForLoop
	{
		public WhileLoop(string value, AbstractExpression logicalCheck)
			: base(value, logicalCheck, null, null)
		{
		}
	}

	public class ForLoop : LogicalFunctionExpression
	{
		public AbstractExpression Initialization { get; }

		public AbstractExpression Incrementation { get; }

		public ForLoop(
			string value,
			AbstractExpression logicalCheck,
			AbstractExpression initialization,
			AbstractExpression incrementation)
			: base(value, logicalCheck)
		{
			Initialization = initialization;
			Incrementation = incrementation;
		}
	}
}