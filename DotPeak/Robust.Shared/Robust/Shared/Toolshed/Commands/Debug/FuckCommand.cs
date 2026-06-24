// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Debug.FuckCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Debug;

[ToolshedCommand]
public sealed class FuckCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public object? Fuck([PipedArgument] object? value) => throw new Exception("fuck!");

  [CommandImplementation(null)]
  public object? Fuck() => throw new Exception("fuck!");
}
