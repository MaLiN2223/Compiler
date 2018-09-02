namespace Compiler.Lexing
{
  public enum TokenType
  {
		/// <summary>
		/// Incorporates all literals that are not of different type e.g. variables
		/// </summary>
    Identifier,

    Operator,

    SimpleType,

    Keyword,

    Punctuation,

    Terminator,

		/// <summary>
		/// Includs things that are keywords but have an assigned type (like true, null etc)
		/// </summary>
    SpecialDataKeyword,

    Scope
  }
}