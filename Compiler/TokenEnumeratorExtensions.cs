using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Lexing;

namespace Compiler
{
	public static class TokenEnumeratorExtensions
	{
		public static IEnumerable<Token> ReadUntil(this IEnumerator<Token> enumerator, Func<Token, bool> delimiter)
		{
			if (delimiter(enumerator.Current))
			{
				yield break;
			}

			while (enumerator.MoveNext() && !delimiter(enumerator.Current))
			{
				yield return enumerator.Current;
			}
		}
		public static IEnumerable<Token> ReadUntilValue(this IEnumerator<Token> enumerator, string value)
		{
			return enumerator.ReadUntil(x => x.Value == value);
		}
	}
}
