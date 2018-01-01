using Compiler.Language;

namespace Compiler.SyntaxTree
{
  public class DeclarationExpression : VariableExpression
  {
    public DeclarationExpression(string type, string name)
      : base(name)
    {
      ValueType = LanguageModel.TypesMap[type];
    }

    public ValueType ValueType { get; }
  }
}