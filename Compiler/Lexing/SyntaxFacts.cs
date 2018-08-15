namespace Compiler.Lexing
{
  public static class SyntaxFacts
  {
    public static bool IsIdentifierPartCharacter(char c)
    {
      return char.IsLetterOrDigit(c);
    }
  }
}
