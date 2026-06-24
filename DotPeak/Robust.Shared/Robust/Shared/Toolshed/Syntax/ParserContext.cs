// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.ParserContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Console;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class ParserContext
{
  public bool NoMultilineExprs;
  public readonly ToolshedManager Toolshed;
  public readonly ToolshedEnvironment Environment;
  public IVariableParser VariableParser;
  public CommandArgumentBundle Bundle;
  public bool GenerateCompletions;
  public CompletionResult? Completions;
  public IConError? Error;
  public readonly string Input;
  public readonly ICommonSession? Session;
  private Stack<Rune> _blockStack = new Stack<Rune>();

  public static bool IsToken(Rune c) => Rune.IsLetterOrDigit(c) || c == new Rune('_');

  public static bool IsCommandToken(Rune c)
  {
    if (Rune.IsLetterOrDigit(c))
      return true;
    return !Rune.IsWhiteSpace(c) && c != new Rune('{') && c != new Rune('}') && c != new Rune('[') && c != new Rune(']') && c != new Rune('(') && c != new Rune(')') && c != new Rune('"') && c != new Rune('\'') && c != new Rune(':') && c != new Rune(';') && c != new Rune('|') && c != new Rune('$') && !Rune.IsControl(c);
  }

  public static bool IsNumeric(Rune c)
  {
    return ParserContext.IsToken(c) || c == new Rune('+') || c == new Rune('-') || c == new Rune('.') || c == new Rune('%');
  }

  public int MaxIndex { get; }

  public int Index { get; private set; }

  public bool OutOfInput => this.Index > this.MaxIndex;

  public ParserContext(
    string input,
    ToolshedManager toolshed,
    ToolshedEnvironment environment,
    IVariableParser parser,
    ICommonSession? session)
  {
    this.Toolshed = toolshed;
    this.Environment = environment;
    this.Input = input;
    this.MaxIndex = input.Length - 1;
    this.VariableParser = parser;
    this.Session = session;
  }

  public ParserContext(string input, ToolshedManager toolshed)
    : this(input, toolshed, toolshed.DefaultEnvironment, IVariableParser.Empty, (ICommonSession) null)
  {
  }

  public ParserContext(string input, ToolshedManager toolshed, IInvocationContext ctx)
    : this(input, toolshed, ctx.Environment, (IVariableParser) new InvocationCtxVarParser(ctx), ctx.Session)
  {
  }

  private ParserContext(ParserContext parserContext, int sliceSize, int? index)
  {
    this.Toolshed = parserContext.Toolshed;
    this.Environment = parserContext.Environment;
    this.Input = parserContext.Input;
    this.Index = index ?? parserContext.Index;
    this.MaxIndex = Math.Min(parserContext.MaxIndex, this.Index + sliceSize - 1);
    this.VariableParser = parserContext.VariableParser;
    this.Session = parserContext.Session;
  }

  public bool EatMatch(char c) => this.EatMatch(new Rune(c));

  public bool EatMatch(Rune c)
  {
    Rune? nullable = this.PeekRune();
    if (!nullable.HasValue || nullable.GetValueOrDefault() != c)
      return false;
    this.Index += c.Utf16SequenceLength;
    return true;
  }

  public bool EatMatch(string c)
  {
    if (this.PeekWord() != c)
      return false;
    this.GetWord();
    return true;
  }

  public char? PeekChar()
  {
    Rune? nullable = this.PeekRune();
    if (!nullable.HasValue)
      return new char?();
    Rune valueOrDefault = nullable.GetValueOrDefault();
    if (valueOrDefault.Utf16SequenceLength > 1)
      return new char?('\u0001');
    Span<char> destination = stackalloc char[2];
    valueOrDefault.EncodeToUtf16(destination);
    return new char?(destination[0]);
  }

  public Rune? PeekRune()
  {
    return this.MaxIndex < this.Index ? new Rune?() : new Rune?(Rune.GetRuneAt(this.Input, this.Index));
  }

  public Rune? GetRune()
  {
    Rune? nullable = this.PeekRune();
    if (!nullable.HasValue)
      return new Rune?();
    Rune valueOrDefault = nullable.GetValueOrDefault();
    this.Index += valueOrDefault.Utf16SequenceLength;
    return new Rune?(valueOrDefault);
  }

  public char? GetChar()
  {
    Rune? nullable = this.PeekRune();
    if (!nullable.HasValue)
      return new char?();
    Rune valueOrDefault = nullable.GetValueOrDefault();
    this.Index += valueOrDefault.Utf16SequenceLength;
    if (valueOrDefault.Utf16SequenceLength > 1)
      return new char?('\u0001');
    Span<char> destination = stackalloc char[2];
    valueOrDefault.EncodeToUtf16(destination);
    return new char?(destination[0]);
  }

  public void DebugPrint()
  {
    Logger.DebugS("parser", string.Join<Rune>(", ", (IEnumerable<Rune>) this._blockStack));
    Logger.DebugS("parser", this.Input);
    this.MakeDebugPointer(this.Index);
    this.MakeDebugPointer(this.MaxIndex, '|');
  }

  private void MakeDebugPointer(int pointAt, char pointer = '^')
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(' ', pointAt);
    stringBuilder.Append(pointer);
    Logger.DebugS("parser", stringBuilder.ToString());
  }

  private string? MaybeGetWord(bool advanceIndex, Func<Rune, bool>? test)
  {
    int index = this.Index;
    if (test == null)
      test = (Func<Rune, bool>) (c => !Rune.IsWhiteSpace(c));
    StringBuilder stringBuilder = new StringBuilder();
    this.ConsumeWhitespace();
    while (true)
    {
      Rune? nullable = this.PeekRune();
      if (nullable.HasValue)
      {
        Rune valueOrDefault = nullable.GetValueOrDefault();
        if (test(valueOrDefault))
          stringBuilder.Append((object) this.GetRune());
        else
          break;
      }
      else
        break;
    }
    if (index == this.Index)
      return (string) null;
    if (!advanceIndex)
      this.Index = index;
    return stringBuilder.ToString();
  }

  public string? PeekWord(Func<Rune, bool>? test = null) => this.MaybeGetWord(false, test);

  public string? GetWord(Func<Rune, bool>? test = null) => this.MaybeGetWord(true, test);

  public bool TryMatch(Regex match, int max = 2147483647 /*0x7FFFFFFF*/)
  {
    ValueList<char> valueList = new ValueList<char>(8);
    Span<char> destination = stackalloc char[4];
    do
    {
      Rune? nullable = this.PeekRune();
      if (!nullable.HasValue)
        return false;
      Rune valueOrDefault = nullable.GetValueOrDefault();
      if (max == 0)
        return false;
      --max;
      int utf16 = valueOrDefault.EncodeToUtf16(destination);
      for (int index = 0; index < utf16; ++index)
        valueList.Add(destination[index]);
    }
    while (!match.IsMatch((ReadOnlySpan<char>) valueList.Span));
    return true;
  }

  public bool TryMatch(string match)
  {
    ValueList<char> valueList = new ValueList<char>(8);
    Span<char> destination = stackalloc char[4];
    int index1 = this.Index;
    int length = match.Length;
    do
    {
      Rune? rune = this.GetRune();
      if (rune.HasValue)
      {
        Rune valueOrDefault = rune.GetValueOrDefault();
        if (length == 0)
        {
          this.Index = index1;
          return false;
        }
        --length;
        int utf16 = valueOrDefault.EncodeToUtf16(destination);
        for (int index2 = 0; index2 < utf16; ++index2)
          valueList.Add(destination[index2]);
      }
      else
      {
        this.Index = index1;
        return false;
      }
    }
    while (!((ReadOnlySpan<char>) valueList.Span).SequenceEqual<char>(match.AsSpan()));
    return true;
  }

  public ParserRestorePoint Save()
  {
    return new ParserRestorePoint(this.Index, new Stack<Rune>((IEnumerable<Rune>) this._blockStack), this.Bundle, this.VariableParser);
  }

  public void Restore(ParserRestorePoint point)
  {
    this.Index = point.Index;
    this._blockStack = point.TerminatorStack;
    this.Bundle = point.Bundle;
    this.VariableParser = point.VariableParser;
  }

  public int ConsumeWhitespace()
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return this.NoMultilineExprs ? this.Consume((Func<Rune, bool>) (x => Rune.IsWhiteSpace(x) && x != new Rune('\n'))) : this.Consume(ParserContext.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace ?? (ParserContext.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace = new Func<Rune, bool>(Rune.IsWhiteSpace)));
  }

  public void PushBlockTerminator(Rune term) => this._blockStack.Push(term);

  public void PushBlockTerminator(char term) => this.PushBlockTerminator(new Rune(term));

  public bool PeekBlockTerminator()
  {
    if (this._blockStack.Count == 0)
      return false;
    Rune? nullable = this.PeekRune();
    Rune rune = this._blockStack.Peek();
    return nullable.HasValue && nullable.GetValueOrDefault() == rune;
  }

  public bool EatBlockTerminator()
  {
    if (this._blockStack.Count == 0 || !this.EatMatch(this._blockStack.Peek()))
      return false;
    this._blockStack.Pop();
    return true;
  }

  public bool CheckEndLine() => this.NoMultilineExprs && this.EatMatch('\n');

  public int Consume(Func<Rune, bool> control)
  {
    int num = 0;
    while (true)
    {
      Rune? nullable = this.PeekRune();
      if (nullable.HasValue)
      {
        Rune valueOrDefault = nullable.GetValueOrDefault();
        if (control(valueOrDefault))
        {
          this.Index += valueOrDefault.Utf16SequenceLength;
          ++num;
        }
        else
          break;
      }
      else
        break;
    }
    return num;
  }

  public ParserContext? SliceBlock(Rune startDelim, Rune endDelim)
  {
    ParserRestorePoint point = this.Save();
    this.ConsumeWhitespace();
    Rune? rune1 = this.GetRune();
    Rune rune2 = startDelim;
    if ((rune1.HasValue ? (rune1.GetValueOrDefault() != rune2 ? 1 : 0) : 1) != 0)
    {
      this.Restore(point);
      return (ParserContext) null;
    }
    int index = this.Index;
    int num = 1;
    while (num > 0)
    {
      Rune? rune3 = this.GetRune();
      Rune? nullable1 = rune3;
      Rune rune4 = startDelim;
      if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() == rune4 ? 1 : 0) : 0) != 0)
        ++num;
      Rune? nullable2 = rune3;
      Rune rune5 = endDelim;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() == rune5 ? 1 : 0) : 0) == 0 || --num != 0)
      {
        if (!rune3.HasValue)
        {
          this.Restore(point);
          return (ParserContext) null;
        }
      }
      else
        break;
    }
    return new ParserContext(this, this.Index - index, new int?(index));
  }

  public bool CheckInvokable(CommandSpec cmd)
  {
    return this.Toolshed.CheckInvokable(cmd, this.Session, out this.Error);
  }

  public bool CheckInvokable<T>() where T : ToolshedCommand
  {
    IReadOnlyList<CommandSpec> commands;
    if (!this.Environment.TryGetCommands<T>(out commands))
      return false;
    foreach (CommandSpec cmd in (IEnumerable<CommandSpec>) commands)
    {
      if (!this.CheckInvokable(cmd))
        return false;
    }
    return true;
  }

  public bool PeekCommandOrBlockTerminated()
  {
    Rune? nullable = this.PeekRune();
    if (!nullable.HasValue)
      return false;
    Rune valueOrDefault = nullable.GetValueOrDefault();
    if (valueOrDefault == new Rune(';') || valueOrDefault == new Rune('|') || this.NoMultilineExprs && valueOrDefault == new Rune('\n'))
      return true;
    return this._blockStack.Count != 0 && valueOrDefault == this._blockStack.Peek();
  }

  public bool EatCommandTerminator(ref Type? pipedType, out bool commandExpected)
  {
    commandExpected = false;
    if (this.EatMatch(new Rune(';')))
    {
      pipedType = (Type) null;
      return true;
    }
    if (pipedType != (Type) null && pipedType != typeof (void) && this.EatMatch(new Rune('|')))
    {
      commandExpected = true;
      return true;
    }
    if (!this.NoMultilineExprs || !this.EatMatch(new Rune('\n')))
      return false;
    pipedType = (Type) null;
    return true;
  }

  public void EatCommandTerminators(ref Type? pipedType, out bool commandExpected)
  {
    if (!this.EatCommandTerminator(ref pipedType, out commandExpected))
      return;
    this.ConsumeWhitespace();
    while (!commandExpected && this.EatCommandTerminator(ref pipedType, out commandExpected))
      this.ConsumeWhitespace();
  }
}
