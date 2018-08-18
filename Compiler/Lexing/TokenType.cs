namespace Compiler.Lexing
{
  public enum TokenType
  {
		/// <summary>
		/// Incorporates all literals that are not of different type e.g. variables
		/// </summary>
    Identifier,

    Operator,

    Type,

    Keyword,

    Punctuation,

    Terminator,

    DataType,

    Scope
  }
}