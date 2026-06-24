// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.CommandInvocationArguments
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed;

internal sealed class CommandInvocationArguments
{
  public required object? PipedArgument;
  public required CommandArgumentBundle Bundle;

  public required IInvocationContext Context { get; set; }

  public Dictionary<string, object?>? Arguments => this.Bundle.Arguments;

  public bool Inverted => this.Bundle.Inverted;
}
