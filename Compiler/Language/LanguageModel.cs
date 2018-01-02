using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Compiler.Language
{
  public static class LanguageModel
  {
    public static readonly char Comment = '#';

    public static readonly string[] DataTypeKeywords = { "true", "false", "null" };

    public static readonly char InstructionTerminator = ';';

    public static readonly string[] KeywordsForScope = { "if", "while", "for" };

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

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBeginingOfIdentifier(char current)
    {
      return char.IsLetter(current) || current == '_';
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDataTypeKeyword(string word)
    {
      return DataTypeKeywords.Contains(word);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDigit(char ch)
    {
      return char.IsDigit(ch);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(char ch)
    {
      return ch == ' ' || ch == '\n' || ch == '\t';
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEof(char ch)
    {
      return ch == '\0';
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInstructionTerminator(char ch)
    {
      return ch == InstructionTerminator;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyword(string word)
    {
      return IsKeywordWithScope(word) || IsDataTypeKeyword(word);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeywordWithScope(string word)
    {
      return KeywordsForScope.Contains(word);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMiddleIdentifier(char current)
    {
      return IsBeginingOfIdentifier(current) || char.IsNumber(current);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOperator(char ch)
    {
      return Operators.Contains(ch);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartOfDigit(char ch)
    {
      return char.IsDigit(ch) || ch == '.';
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPunctuation(char ch)
    {
      return Punctuations.Contains(ch);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsscopeIndicator(char ch)
    {
      return ch == '}' || ch == '{';
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsType(string ch)
    {
      return Types.Contains(ch);
    }
  }
}