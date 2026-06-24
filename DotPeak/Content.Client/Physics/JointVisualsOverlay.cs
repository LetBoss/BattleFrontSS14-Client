// Decompiled with JetBrains decompiler
// Type: Content.Client.Physics.JointVisualsOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Physics;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Physics;

public sealed class JointVisualsOverlay : Overlay
{
  private IEntityManager _entManager;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public JointVisualsOverlay(IEntityManager entManager) => this._entManager = entManager;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    SpriteSystem spriteSystem = this._entManager.System<SpriteSystem>();
    SharedTransformSystem sharedTransformSystem = this._entManager.System<SharedTransformSystem>();
    EntityQueryEnumerator<JointVisualsComponent, TransformComponent> entityQueryEnumerator = this._entManager.EntityQueryEnumerator<JointVisualsComponent, TransformComponent>();
    EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
    DrawingHandleBase drawingHandle = args.DrawingHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    drawingHandle.SetTransform(ref local);
    JointVisualsComponent visualsComponent;
    TransformComponent transformComponent1;
    while (entityQueryEnumerator.MoveNext(ref visualsComponent, ref transformComponent1))
    {
      if (!MapId.op_Inequality(transformComponent1.MapID, args.MapId))
      {
        EntityUid? entity = this._entManager.GetEntity(visualsComponent.Target);
        TransformComponent transformComponent2;
        if (entityQuery.TryGetComponent(entity, ref transformComponent2) && !MapId.op_Inequality(transformComponent1.MapID, transformComponent2.MapID))
        {
          Texture texture = spriteSystem.Frame0(visualsComponent.Sprite);
          float num1 = (float) texture.Width / 32f;
          EntityCoordinates coordinates1 = transformComponent1.Coordinates;
          EntityCoordinates coordinates2 = transformComponent2.Coordinates;
          Angle localRotation1 = transformComponent1.LocalRotation;
          Angle localRotation2 = transformComponent2.LocalRotation;
          EntityCoordinates entityCoordinates1 = ((EntityCoordinates) ref coordinates1).Offset(((Angle) ref localRotation1).RotateVec(ref visualsComponent.OffsetA));
          EntityCoordinates entityCoordinates2 = ((EntityCoordinates) ref coordinates2).Offset(((Angle) ref localRotation2).RotateVec(ref visualsComponent.OffsetB));
          Vector2 position1 = sharedTransformSystem.ToMapCoordinates(entityCoordinates1, true).Position;
          Vector2 position2 = sharedTransformSystem.ToMapCoordinates(entityCoordinates2, true).Position;
          Vector2 vector2_1 = position2 - position1;
          float num2 = vector2_1.Length();
          Vector2 vector2_2 = vector2_1 / 2f + position1;
          Angle worldAngle = DirectionExtensions.ToWorldAngle(position2 - position1);
          Box2 box2;
          // ISSUE: explicit constructor call
          ((Box2) ref box2).\u002Ector((float) (-(double) num1 / 2.0), (float) (-(double) num2 / 2.0), num1 / 2f, num2 / 2f);
          Box2Rotated box2Rotated;
          // ISSUE: explicit constructor call
          ((Box2Rotated) ref box2Rotated).\u002Ector(((Box2) ref box2).Translated(vector2_2), worldAngle, vector2_2);
          worldHandle.DrawTextureRect(texture, ref box2Rotated, new Color?());
        }
      }
    }
  }
}
