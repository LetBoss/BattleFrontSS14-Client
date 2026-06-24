// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Values.ValCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand]
public sealed class ValCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (TypeTypeParser)
  };

  public override Type[] TypeParameterParsers => ValCommand._parsers;

  [CommandImplementation(null)]
  public T Val<T>(T value) => value;
}
