// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.World.PosCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities.World;

[ToolshedCommand]
internal sealed class PosCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public EntityCoordinates Pos([PipedArgument] EntityUid ent) => this.Transform(ent).Coordinates;

  [CommandImplementation(null)]
  public IEnumerable<EntityCoordinates> Pos([PipedArgument] IEnumerable<EntityUid> input)
  {
    return input.Select<EntityUid, EntityCoordinates>(new Func<EntityUid, EntityCoordinates>(this.Pos));
  }

  [CommandImplementation(null)]
  public EntityCoordinates Pos(IInvocationContext ctx)
  {
    EntityUid? nullable = this.ExecutingEntity(ctx);
    return nullable.HasValue ? this.Transform(nullable.GetValueOrDefault()).Coordinates : EntityCoordinates.Invalid;
  }
}
