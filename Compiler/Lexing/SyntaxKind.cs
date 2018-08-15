namespace Compiler.Lexing
{
  public enum SyntaxKind : ushort
  {
    None = 0,


    LiteralTrueKeyword = 100,
    LiteralFalseKeyword = 101,
    ReturnKeyword,
    VarKeyword,

    // Types
    StringKeyword,
    IntKeyword,
    BoolKeyword,
    ByteKeyword,
    DoubleKeyword,
    CharKeyword,

    // Tokens
    PlusToken,
    MinusToken,
    EqualsToken,
    AsteriskToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    OpenBracketToken,
    CloseBracketToken,
    SemicolonToken,

    // Literals
    
    NumericLiteralExpression,
    StringLiteralExpression
  }
}
