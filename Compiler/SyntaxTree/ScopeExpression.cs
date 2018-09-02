using System;
using System.Collections.Generic;

namespace Compiler.SyntaxTree
{
	public class FunctionExpression : ScopeExpression
	{
		public string Name { get; set; }
		public string ReturnType { get; set; } 
		public List<Tuple<string, string>> Arguments { get; set; }
		protected override string DoDump()
		{
			string output = "";
			foreach (var expression in Expressions)
			{
				output += $"\n{expression.Dump()}";
			}

			return output;
		}
	}


	public class ScopeExpression : AbstractExpression
	{
		public List<AbstractExpression> Expressions = new List<AbstractExpression>();

		protected override string DoDump()
		{
			string output = "";
			foreach (var expression in Expressions)
			{

				output += $"{expression.Dump()}";
			}

			return output;
		}
	}
}