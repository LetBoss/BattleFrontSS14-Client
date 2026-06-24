// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Atgm.CivAtgmWireOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Atgm;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Atgm;

public sealed class CivAtgmWireOverlay : Overlay
{
  [Dependency]
  private readonly IEntityManager _entity;

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public CivAtgmWireOverlay() => IoCManager.InjectDependencies<CivAtgmWireOverlay>(this);

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    TransformSystem transformSystem = this._entity.System<TransformSystem>();
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQueryEnumerator<CivAtgmWireComponent, TransformComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<CivAtgmWireComponent, TransformComponent>();
    EntityUid entityUid;
    CivAtgmWireComponent atgmWireComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref atgmWireComponent, ref transformComponent))
    {
      NetCoordinates? origin = atgmWireComponent.Origin;
      if (origin.HasValue)
      {
        NetCoordinates valueOrDefault = origin.GetValueOrDefault();
        if (!MapId.op_Equality(transformComponent.MapID, MapId.Nullspace) && !MapId.op_Inequality(transformComponent.MapID, args.MapId))
        {
          MapCoordinates mapCoordinates1 = ((SharedTransformSystem) transformSystem).ToMapCoordinates(this._entity.GetCoordinates(valueOrDefault), true);
          if (!MapId.op_Inequality(mapCoordinates1.MapId, args.MapId))
          {
            MapCoordinates mapCoordinates2 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(entityUid, (TransformComponent) null);
            ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates1.Position, mapCoordinates2.Position, Color.FromHex((ReadOnlySpan<char>) "#3A3A3A", new Color?()));
          }
        }
      }
    }
  }
}
