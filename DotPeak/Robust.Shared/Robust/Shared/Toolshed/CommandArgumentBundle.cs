// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.CommandArgumentBundle
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed;

public struct CommandArgumentBundle
{
  public string? Command;
  public string? SubCommand;
  public Dictionary<string, object?>? Arguments;
  public Type[]? TypeArguments;
  public required bool Inverted;
  public required Type? PipedType;
  public int NameStart;
  public int NameEnd;
}
