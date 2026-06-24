// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.PrototypedCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class PrototypedCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public IEnumerable<EntityUid> Prototyped(
    [PipedArgument] IEnumerable<EntityUid> input,
    EntProtoId prototype,
    [CommandInverted] bool inverted)
  {
    return input.Where<EntityUid>((Func<EntityUid, bool>) (x => this.MetaData(x).EntityPrototype?.ID == prototype.Id ^ inverted));
  }
}
