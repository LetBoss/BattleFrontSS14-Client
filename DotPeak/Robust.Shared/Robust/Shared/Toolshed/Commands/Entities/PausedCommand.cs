// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.PausedCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class PausedCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public IEnumerable<EntityUid> Paused([PipedArgument] IEnumerable<EntityUid> entities, [CommandInverted] bool inverted)
  {
    return entities.Where<EntityUid>((Func<EntityUid, bool>) (x => this.Comp<MetaDataComponent>(x).EntityPaused ^ inverted));
  }
}
