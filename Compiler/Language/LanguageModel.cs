using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Compiler.Language
{
	public class LanguageModel : ILanguageModel
	{
		public static readonly char Comment = '#';

		public static readonly string[] DataTypeKeywords = { "true", "false", "null" };

		public static readonly char InstructionTerminator = ';';

		public static readonly string[] KeywordsForScope = { "if", "while", "for", "else" };

		public static readonly char[] Operators = { '+', '-', '/', '*', '%', '!', '<', '>', '=', '^' };

		public static readonly char[] Punctuations = { ',', ')', '(' };

		public static readonly string[] Types;

		public static readonly Dictionary<string, ValueType> TypesMap =
			new Dictionary<string, ValueType>
				{
					{ "int", ValueType.Int },
					{ "tribool", ValueType.Tribool },
					{ "double", ValueType.Double },
					{ "bool", ValueType.Bool }
				};

		public static readonly Dictionary<ValueType, Type> ValueTypeToTypeMap =
			new Dictionary<ValueType, Type>
				{
					{ ValueType.Int, typeof(int) },
					{ ValueType.Tribool, typeof(bool?) },
					{ ValueType.Double, typeof(double) },
					{ ValueType.Bool, typeof(bool) }
				};

		static LanguageModel()
		{
			Types = TypesMap.Keys.ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsCommentCharacter(char c)
		{
			return Comment == c;
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsBeginingOfIdentifier(char current)
		{
			return char.IsLetter(current) || current == '_';
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsDataTypeKeyword(string word)
		{
			return DataTypeKeywords.Contains(word);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsDigit(char ch)
		{
			return char.IsDigit(ch);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsEmpty(char ch)
		{
			return ch == ' ' || ch == '\n' || ch == '\t';
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsEof(char ch)
		{
			return ch == '\0';
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsInstructionTerminator(char ch)
		{
			return ch == InstructionTerminator;
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsKeyword(string word)
		{
			return IsKeywordWithScope(word) || IsDataTypeKeyword(word);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsKeywordWithScope(string word)
		{
			return KeywordsForScope.Contains(word);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsMiddleIdentifier(char current)
		{
			return IsBeginingOfIdentifier(current) || char.IsNumber(current);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsOperator(char ch)
		{
			return Operators.Contains(ch);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsPartOfDigit(char ch)
		{
			return char.IsDigit(ch) || ch == '.';
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsPunctuation(char ch)
		{
			return Punctuations.Contains(ch);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsscopeIndicator(char ch)
		{
			return ch == '}' || ch == '{';
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsType(string ch)
		{
			return Types.Contains(ch);
		}
	}
}