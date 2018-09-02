using System;
using System.Collections;
using System.Collections.Generic;
using Compiler.Language;

namespace Compiler.Lexing
{
	public abstract class StringDataEnumerator : IEnumerable<char>
	{

		public StringDataEnumerator()
		{
		}

		protected IEnumerable<char> EnumerateLines(IEnumerable<string> lines)
		{
			bool isFirstLine = true;
			foreach (var line in lines)
			{
				if (!isFirstLine)
				{
					yield return '\n'; //New line
				}
				foreach (var x in GetEnumeratorForLine(line))
				{
					yield return x;
				}

				isFirstLine = false;
			}

			yield return '\0';
		}
		private IEnumerable<char> GetEnumeratorForLine(string line)
		{
			var lineEnumerator = line.GetEnumerator();
			foreach (var ch in Iterate(lineEnumerator))
			{
				yield return ch;
			}
		}

		private IEnumerable<char> Iterate(CharEnumerator source)
		{
			while (source.MoveNext())
			{
				yield return source.Current;
			}
		}


		public abstract IEnumerator<char> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}