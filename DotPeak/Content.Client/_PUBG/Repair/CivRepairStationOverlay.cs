// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Repair.CivRepairStationOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Repair;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._PUBG.Repair;

public sealed class CivRepairStationOverlay : Overlay
{
  [Dependency]
  private readonly IEntityManager _entity;

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public CivRepairStationOverlay() => IoCManager.InjectDependencies<CivRepairStationOverlay>(this);

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    TransformSystem transformSystem = this._entity.System<TransformSystem>();
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQueryEnumerator<CivRepairStationComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<CivRepairStationComponent>();
    EntityUid entityUid;
    CivRepairStationComponent stationComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref stationComponent))
    {
      EntityUid? welder = stationComponent.Welder;
      if (welder.HasValue)
      {
        EntityUid valueOrDefault = welder.GetValueOrDefault();
        TransformComponent transformComponent1;
        TransformComponent transformComponent2;
        if (((EntityUid) ref valueOrDefault).Valid && this._entity.TryGetComponent<TransformComponent>(entityUid, ref transformComponent1) && !MapId.op_Equality(transformComponent1.MapID, MapId.Nullspace) && this._entity.TryGetComponent<TransformComponent>(valueOrDefault, ref transformComponent2) && !MapId.op_Equality(transformComponent2.MapID, MapId.Nullspace))
        {
          MapCoordinates mapCoordinates1 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(entityUid, (TransformComponent) null);
          MapCoordinates mapCoordinates2 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(valueOrDefault, (TransformComponent) null);
          if (!MapId.op_Inequality(mapCoordinates1.MapId, mapCoordinates2.MapId))
            ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates1.Position, mapCoordinates2.Position, Color.Black);
        }
      }
    }
  }
}
