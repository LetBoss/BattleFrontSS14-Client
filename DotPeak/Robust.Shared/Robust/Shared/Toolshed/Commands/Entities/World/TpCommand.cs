// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.World.TpCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities.World;

[ToolshedCommand]
internal sealed class TpCommand : ToolshedCommand
{
  private SharedTransformSystem? _xform;

  [CommandImplementation("coords")]
  public EntityUid TpCoords([PipedArgument] EntityUid teleporter, [CommandArgument(null, true)] EntityCoordinates target)
  {
    if (this._xform == null)
      this._xform = this.GetSys<SharedTransformSystem>();
    this._xform.SetCoordinates(teleporter, target);
    return teleporter;
  }

  [CommandImplementation("coords")]
  public IEnumerable<EntityUid> TpCoords(
    [PipedArgument] IEnumerable<EntityUid> teleporters,
    [CommandArgument(null, true)] EntityCoordinates target)
  {
    return teleporters.Select<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (x => this.TpCoords(x, target)));
  }

  [CommandImplementation("to")]
  public EntityUid TpTo([PipedArgument] EntityUid teleporter, EntityUid target)
  {
    if (this._xform == null)
      this._xform = this.GetSys<SharedTransformSystem>();
    this._xform.SetCoordinates(teleporter, this.Transform(target).Coordinates);
    return teleporter;
  }

  [CommandImplementation("to")]
  public IEnumerable<EntityUid> TpTo([PipedArgument] IEnumerable<EntityUid> teleporters, EntityUid target)
  {
    return teleporters.Select<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (x => this.TpTo(x, target)));
  }

  [CommandImplementation("into")]
  public EntityUid TpInto([PipedArgument] EntityUid teleporter, EntityUid target)
  {
    if (this._xform == null)
      this._xform = this.GetSys<SharedTransformSystem>();
    this._xform.SetCoordinates(teleporter, new EntityCoordinates(target, Vector2.Zero));
    return teleporter;
  }

  [CommandImplementation("into")]
  public IEnumerable<EntityUid> TpInto([PipedArgument] IEnumerable<EntityUid> teleporters, EntityUid target)
  {
    return teleporters.Select<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (x => this.TpInto(x, target)));
  }
}
