// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.AsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class AsCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (TypeTypeParser)
  };

  public override Type[] TypeParameterParsers => AsCommand._parsers;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public TOut? As<TOut, TIn>([PipedArgument] TIn value) => (TOut) (object) value;
}
