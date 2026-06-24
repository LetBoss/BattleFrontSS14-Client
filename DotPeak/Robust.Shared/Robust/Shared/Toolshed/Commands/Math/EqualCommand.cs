// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.EqualCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "==")]
public sealed class EqualCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public bool Comparison<T>([PipedArgument] T x, T y) where T : IEquatable<T> => x.Equals(y);
}
