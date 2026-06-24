// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.CompletionOptionFlags
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Console;

[Flags]
public enum CompletionOptionFlags
{
  PartialCompletion = 1,
  NoQuote = 2,
  NoEscape = 4,
}
