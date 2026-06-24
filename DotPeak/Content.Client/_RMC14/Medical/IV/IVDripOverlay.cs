// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.IV.IVDripOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Medical.IV;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._RMC14.Medical.IV;

public sealed class IVDripOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public IVDripOverlay() => IoCManager.InjectDependencies<IVDripOverlay>(this);

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    TransformSystem transformSystem = this._entity.System<TransformSystem>();
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQueryEnumerator<IVDripComponent> entityQueryEnumerator1 = this._entity.EntityQueryEnumerator<IVDripComponent>();
    EntityUid entityUid1;
    IVDripComponent ivDripComponent;
    EntityUid? attachedTo;
    while (entityQueryEnumerator1.MoveNext(ref entityUid1, ref ivDripComponent))
    {
      attachedTo = ivDripComponent.AttachedTo;
      if (attachedTo.HasValue)
      {
        EntityUid valueOrDefault = attachedTo.GetValueOrDefault();
        if (((EntityUid) ref valueOrDefault).Valid)
        {
          MapCoordinates mapCoordinates1 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(entityUid1, (TransformComponent) null);
          MapCoordinates mapCoordinates2 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(valueOrDefault, (TransformComponent) null);
          if (!MapId.op_Equality(mapCoordinates1.MapId, MapId.Nullspace) && !MapId.op_Equality(mapCoordinates2.MapId, MapId.Nullspace))
            ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates1.Position, mapCoordinates2.Position, Color.White);
        }
      }
    }
    EntityQueryEnumerator<BloodPackComponent> entityQueryEnumerator2 = this._entity.EntityQueryEnumerator<BloodPackComponent>();
    EntityUid entityUid2;
    BloodPackComponent bloodPackComponent;
    while (entityQueryEnumerator2.MoveNext(ref entityUid2, ref bloodPackComponent))
    {
      attachedTo = bloodPackComponent.AttachedTo;
      if (attachedTo.HasValue)
      {
        EntityUid valueOrDefault = attachedTo.GetValueOrDefault();
        if (((EntityUid) ref valueOrDefault).Valid)
        {
          MapCoordinates mapCoordinates3 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(entityUid2, (TransformComponent) null);
          MapCoordinates mapCoordinates4 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(valueOrDefault, (TransformComponent) null);
          if (!MapId.op_Equality(mapCoordinates3.MapId, MapId.Nullspace) && !MapId.op_Equality(mapCoordinates4.MapId, MapId.Nullspace))
            ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates3.Position, mapCoordinates4.Position, Color.White);
        }
      }
    }
  }
}
