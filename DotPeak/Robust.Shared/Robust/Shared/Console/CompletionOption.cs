// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.CompletionOption
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Console;

public record struct CompletionOption(string Value, string? Hint = null, CompletionOptionFlags Flags = (CompletionOptionFlags) 0) : 
  IComparable<CompletionOption>
{
  public int CompareTo(CompletionOption other)
  {
    return string.Compare(this.Value, other.Value, StringComparison.CurrentCultureIgnoreCase);
  }
}
