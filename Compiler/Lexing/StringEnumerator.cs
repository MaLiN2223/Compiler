using System;
using System.Collections.Generic;
using Compiler.Language;

namespace Compiler.Lexing
{
	public class StringEnumerator : StringDataEnumerator
	{
		private readonly string Text;
		public StringEnumerator(string @string)
		{
			Text = @string;
		}
		public override IEnumerator<char> GetEnumerator()
		{
			var splitted = Text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
			return EnumerateLines(splitted).GetEnumerator();
		}
	}
}