// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.TextParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.IO;

#nullable enable
namespace Robust.Shared.Utility;

internal sealed class TextParser
{
  private readonly TextReader _reader;
  private string? _currentLine;

  public int CurrentLine { get; private set; }

  public int CurrentIndex { get; private set; }

  public TextParser(TextReader reader) => this._reader = reader;

  public void NextLine()
  {
    this._currentLine = this._reader.ReadLine();
    this.CurrentIndex = 0;
    ++this.CurrentLine;
  }

  public bool TryParse(string str)
  {
    if (this.IsEOL())
      return false;
    int num = this._currentLine.IndexOf(str, this.CurrentIndex, StringComparison.Ordinal) == this.CurrentIndex ? 1 : 0;
    if (num == 0)
      return num != 0;
    this.Advance(str.Length);
    return num != 0;
  }

  public bool TryParse(char chr)
  {
    if (this.IsEOL())
      return false;
    int num = (int) this._currentLine[this.CurrentIndex] == (int) chr ? 1 : 0;
    if (num == 0)
      return num != 0;
    this.Advance();
    return num != 0;
  }

  public void Parse(char chr)
  {
    if (this.IsEOL())
      throw new TextParser.ParserException($"Expected '{chr}', got EOL");
    if ((int) this._currentLine[this.CurrentIndex] != (int) chr)
      throw new TextParser.ParserException($"Expected '{chr}'.");
    this.Advance();
  }

  public bool IsEOL() => this._currentLine == null || this._currentLine.Length <= this.CurrentIndex;

  public bool IsEOF() => this._reader.Peek() == -1 && this.IsEOL();

  public void EnsureEOL()
  {
    if (!this.IsEOL())
      throw new TextParser.ParserException("Expected EOL");
  }

  public void EnsureNoEOL()
  {
    if (this.IsEOL())
      throw new TextParser.ParserException("Unexpected EOL");
  }

  public bool EatWhitespace()
  {
    bool flag = false;
    while (!this.IsEOL() && char.IsWhiteSpace(this._currentLine, this.CurrentIndex))
    {
      this.Advance();
      flag = true;
    }
    return flag;
  }

  public string EatUntilWhitespace()
  {
    int currentIndex = this.CurrentIndex;
    while (!this.IsEOL() && !char.IsWhiteSpace(this._currentLine, this.CurrentIndex))
      this.Advance();
    return this._currentLine.Substring(currentIndex, this.CurrentIndex - currentIndex);
  }

  public string EatUntilEOL()
  {
    int currentIndex = this.CurrentIndex;
    while (!this.IsEOL())
      this.Advance();
    return this._currentLine.Substring(currentIndex, this.CurrentIndex - currentIndex);
  }

  public char Peek()
  {
    return this.CurrentIndex >= this._currentLine.Length ? char.MinValue : this._currentLine[this.CurrentIndex];
  }

  public char Take() => this._currentLine[this.CurrentIndex++];

  public void Advance(int amount = 1) => this.CurrentIndex += amount;

  public bool PeekIsDigit() => char.IsDigit(this._currentLine, this.CurrentIndex);

  [Virtual]
  public class ParserException : Exception
  {
    public ParserException(string message)
      : base(message)
    {
    }

    public ParserException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
