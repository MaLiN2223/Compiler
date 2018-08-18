namespace Compiler.Language
{
	public interface ILanguageModel
	{
		bool IsPunctuation(char ch);
		bool IsType(string ch);
		bool IsCommentCharacter(char c);
		bool IsEof(char ch);
		bool IsEmpty(char ch);
		bool IsDigit(char ch);
		bool IsBeginingOfIdentifier(char ch);
		bool IsMiddleIdentifier(char ch);
		bool IsDataTypeKeyword(string ch);
		bool IsInstructionTerminator(char ch);
		bool IsKeyword(string ch);
		bool IsKeywordWithScope(string str);
		bool IsOperator(char ch);
		bool IsPartOfDigit(char ch);
		bool IsScopeIndicator(char ch);
	}
}