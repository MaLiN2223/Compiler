namespace Compiler.Lexing
{
  public struct Token
  {
    public Token(string value, TokenType type, SyntaxKind kind = SyntaxKind.None)
    {
      Value = value;
      Type = type;
      SyntaxKind = kind;
    }

    public TokenType Type { get; set; }

    public string Value { get; set; }

    public SyntaxKind SyntaxKind { get; set; }
  }
}