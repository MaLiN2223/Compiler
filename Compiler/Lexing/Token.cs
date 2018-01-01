namespace Compiler.Lexing
{
  public struct Token
  {
    public Token(string value, TokenType type)
    {
      Value = value;
      Type = type;
    }

    public TokenType Type { get; set; }

    public string Value { get; set; }
  }
}