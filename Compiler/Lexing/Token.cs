namespace Compiler.Lexing
{
	public struct Token
	{
		public Token(string value, TokenType type, int line, SyntaxKind kind = SyntaxKind.None)
		{
			Value = value;
			Type = type;
			SyntaxKind = kind;
			Line = line;
		}
		public int Line { get; set; }

		public TokenType Type { get; set; }

		public string Value { get; set; }

		public SyntaxKind SyntaxKind { get; set; }
	}
}