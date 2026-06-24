// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Telephone.TelephoneOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Telephone;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._RMC14.Telephone;

public sealed class TelephoneOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entity;

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public TelephoneOverlay() => IoCManager.InjectDependencies<TelephoneOverlay>(this);

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    ContainerSystem containerSystem = this._entity.System<ContainerSystem>();
    TransformSystem transformSystem = this._entity.System<TransformSystem>();
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQueryEnumerator<RotaryPhoneComponent> entityQueryEnumerator = this._entity.EntityQueryEnumerator<RotaryPhoneComponent>();
    EntityUid entityUid;
    RotaryPhoneComponent rotaryPhoneComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref rotaryPhoneComponent))
    {
      EntityUid? phone = rotaryPhoneComponent.Phone;
      if (phone.HasValue)
      {
        EntityUid valueOrDefault = phone.GetValueOrDefault();
        BaseContainer baseContainer;
        TransformComponent transformComponent1;
        TransformComponent transformComponent2;
        if (((EntityUid) ref valueOrDefault).Valid && ((SharedContainerSystem) containerSystem).TryGetContainer(entityUid, rotaryPhoneComponent.ContainerId, ref baseContainer, (ContainerManagerComponent) null) && baseContainer.ContainedEntities.Count <= 0 && this._entity.TryGetComponent<TransformComponent>(entityUid, ref transformComponent1) && !MapId.op_Equality(transformComponent1.MapID, MapId.Nullspace) && this._entity.TryGetComponent<TransformComponent>(valueOrDefault, ref transformComponent2) && !MapId.op_Equality(transformComponent2.MapID, MapId.Nullspace))
        {
          MapCoordinates mapCoordinates1 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(entityUid, (TransformComponent) null);
          MapCoordinates mapCoordinates2 = ((SharedTransformSystem) transformSystem).GetMapCoordinates(valueOrDefault, (TransformComponent) null);
          ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates1.Position, mapCoordinates2.Position, Color.Black);
        }
      }
    }
  }
}
