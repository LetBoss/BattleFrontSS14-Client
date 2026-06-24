// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Targeting.TargetingOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Targeting;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Targeting;

public sealed class TargetingOverlay : Overlay
{
  private IEntityManager _entManager;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public TargetingOverlay(IEntityManager entManager) => this._entManager = entManager;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityQueryEnumerator<RMCTargetedComponent> entityQueryEnumerator = this._entManager.EntityQueryEnumerator<RMCTargetedComponent>();
    EntityQuery<TransformComponent> entityQuery1 = this._entManager.GetEntityQuery<TransformComponent>();
    EntityQuery<TargetingLaserComponent> entityQuery2 = this._entManager.GetEntityQuery<TargetingLaserComponent>();
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    SharedTransformSystem sharedTransformSystem = this._entManager.System<SharedTransformSystem>();
    EntityUid entityUid;
    RMCTargetedComponent targetedComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref targetedComponent))
    {
      foreach (EntityUid key in targetedComponent.TargetedBy)
      {
        TargetingLaserComponent targetingLaserComponent;
        TransformComponent transformComponent1;
        TransformComponent transformComponent2;
        if (entityQuery2.TryGetComponent(key, ref targetingLaserComponent) && targetingLaserComponent.ShowLaser && entityQuery1.TryGetComponent(key, ref transformComponent1) && entityQuery1.TryGetComponent(entityUid, ref transformComponent2) && !MapId.op_Inequality(transformComponent2.MapID, transformComponent1.MapID))
        {
          Vector2 worldPosition1 = sharedTransformSystem.GetWorldPosition(transformComponent2, entityQuery1);
          Vector2 worldPosition2 = sharedTransformSystem.GetWorldPosition(transformComponent1, entityQuery1);
          Vector2 vector2_1 = worldPosition1 - worldPosition2;
          Angle worldAngle = DirectionExtensions.ToWorldAngle(vector2_1);
          float num1 = vector2_1.Length() / 2f;
          Vector2 vector2_2 = worldPosition2 + vector2_1 / 2f;
          Box2 box2;
          // ISSUE: explicit constructor call
          ((Box2) ref box2).\u002Ector(-0.02f, -num1, 0.02f, num1);
          Box2Rotated box2Rotated;
          // ISSUE: explicit constructor call
          ((Box2Rotated) ref box2Rotated).\u002Ector(((Box2) ref box2).Translated(vector2_2), worldAngle, vector2_2);
          Color currentLaserColor = targetingLaserComponent.CurrentLaserColor;
          float num2 = 0.0f;
          if (targetingLaserComponent.GradualAlpha)
          {
            float num3;
            if (targetedComponent.AlphaMultipliers.TryGetValue(key, out num3))
              num2 = targetingLaserComponent.LaserAlpha * num3;
          }
          else
            num2 = targetingLaserComponent.LaserAlpha;
          worldHandle.DrawRect(ref box2Rotated, ((Color) ref currentLaserColor).WithAlpha(num2), true);
        }
      }
    }
  }
}
