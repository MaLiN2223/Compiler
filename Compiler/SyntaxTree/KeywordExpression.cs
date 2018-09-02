namespace Compiler.SyntaxTree
{
	public class KeywordExpression : AbstractExpression
	{
		public KeywordExpression(string value)
		{
			Value = value;
		}


		public string Value { get; }
	}

	public interface IScopedExpression
	{
		ScopeExpression BodyExpression { get; set; }
	}
}