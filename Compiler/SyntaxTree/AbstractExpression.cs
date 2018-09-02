using System;
using System.Collections.Generic;

namespace Compiler.SyntaxTree
{
	public abstract class AbstractExpression
	{
		public IEnumerable<AbstractExpression> Children { get; } = new List<AbstractExpression>();
		protected virtual string DoDump()
		{
			return GetType().FullName;
		}

		public string Dump()
		{
			return DoDump();
		}
	}
}