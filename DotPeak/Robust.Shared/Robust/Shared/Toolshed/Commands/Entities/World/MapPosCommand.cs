// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.World.MapPosCommand
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
internal sealed class MapPosCommand : ToolshedCommand
{
  private SharedTransformSystem? _xform;

  [CommandImplementation(null)]
  public EntityCoordinates MapPos([PipedArgument] EntityUid ent)
  {
    this._xform = this.GetSys<SharedTransformSystem>();
    TransformComponent component = this.Transform(ent);
    Vector2 worldPosition = this._xform.GetWorldPosition(component);
    return new EntityCoordinates(component.MapUid ?? EntityUid.Invalid, worldPosition);
  }

  [CommandImplementation(null)]
  public IEnumerable<EntityCoordinates> MapPos([PipedArgument] IEnumerable<EntityUid> input)
  {
    return input.Select<EntityUid, EntityCoordinates>(new Func<EntityUid, EntityCoordinates>(this.MapPos));
  }
}
