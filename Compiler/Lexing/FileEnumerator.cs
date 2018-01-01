using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Compiler.Language;

namespace Compiler.Lexing
{
  public class FileEnumerator : IEnumerable<char>
  {
    private readonly string _file;

    public FileEnumerator(string file)
    {
      _file = file;
    }

    public IEnumerator<char> GetEnumerator()
    {
      return SourceEnumerator(_file);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private IEnumerable<char> Iterate(CharEnumerator source)
    {
      while (source.MoveNext())
      {
        var ch = source.Current;
        if (ch == LanguageModel.Comment)
        {
          yield break;
        }

        yield return ch;
      }
    }

    private IEnumerator<char> SourceEnumerator(string file)
    {
      var f = File.ReadAllLines(file);
      foreach (var line in f)
      {
        var lineEnumerator = line.GetEnumerator();
        foreach (var ch in Iterate(lineEnumerator))
        {
          yield return ch;
        }
      }

      yield return '\0';
    }
  }
}