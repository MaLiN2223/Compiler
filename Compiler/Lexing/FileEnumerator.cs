using System.Collections.Generic;
using System.IO;
using Compiler.Language;

namespace Compiler.Lexing
{
	public class FileEnumerator : StringDataEnumerator
	{
		private readonly string _file;

		public FileEnumerator(string file)
		{
			_file = file;
		}

		public override IEnumerator<char> GetEnumerator()
		{
			return SourceEnumerator(_file);
		}

		private IEnumerator<char> SourceEnumerator(string file)
		{
			return EnumerateLines(File.ReadLines(file)).GetEnumerator();
		}
	}
}