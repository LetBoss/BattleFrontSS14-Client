// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Misc.TetherGunOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Weapons.Misc;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Misc;

public sealed class TetherGunOverlay : Overlay
{
  private IEntityManager _entManager;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public TetherGunOverlay(IEntityManager entManager) => this._entManager = entManager;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityQueryEnumerator<TetheredComponent> entityQueryEnumerator = this._entManager.EntityQueryEnumerator<TetheredComponent>();
    EntityQuery<TransformComponent> entityQuery1 = this._entManager.GetEntityQuery<TransformComponent>();
    EntityQuery<TetherGunComponent> entityQuery2 = this._entManager.GetEntityQuery<TetherGunComponent>();
    EntityQuery<ForceGunComponent> entityQuery3 = this._entManager.GetEntityQuery<ForceGunComponent>();
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    SharedTransformSystem sharedTransformSystem = this._entManager.System<SharedTransformSystem>();
    EntityUid entityUid;
    TetheredComponent tetheredComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref tetheredComponent))
    {
      EntityUid tetherer = tetheredComponent.Tetherer;
      TransformComponent transformComponent1;
      TransformComponent transformComponent2;
      if (entityQuery1.TryGetComponent(tetherer, ref transformComponent1) && entityQuery1.TryGetComponent(entityUid, ref transformComponent2) && !MapId.op_Inequality(transformComponent2.MapID, transformComponent1.MapID))
      {
        Vector2 worldPosition1 = sharedTransformSystem.GetWorldPosition(transformComponent2, entityQuery1);
        Vector2 worldPosition2 = sharedTransformSystem.GetWorldPosition(transformComponent1, entityQuery1);
        Vector2 vector2_1 = worldPosition1 - worldPosition2;
        Angle worldAngle = DirectionExtensions.ToWorldAngle(vector2_1);
        float num = vector2_1.Length() / 2f;
        Vector2 vector2_2 = worldPosition2 + vector2_1 / 2f;
        Box2 box2;
        // ISSUE: explicit constructor call
        ((Box2) ref box2).\u002Ector(-0.05f, -num, 0.05f, num);
        Box2Rotated box2Rotated;
        // ISSUE: explicit constructor call
        ((Box2Rotated) ref box2Rotated).\u002Ector(((Box2) ref box2).Translated(vector2_2), worldAngle, vector2_2);
        Color color = Color.Red;
        ForceGunComponent forceGunComponent;
        if (entityQuery3.TryGetComponent(tetheredComponent.Tetherer, ref forceGunComponent))
        {
          color = forceGunComponent.LineColor;
        }
        else
        {
          TetherGunComponent tetherGunComponent;
          if (entityQuery2.TryGetComponent(tetheredComponent.Tetherer, ref tetherGunComponent))
            color = tetherGunComponent.LineColor;
        }
        worldHandle.DrawRect(ref box2Rotated, ((Color) ref color).WithAlpha(0.3f), true);
      }
    }
  }
}
