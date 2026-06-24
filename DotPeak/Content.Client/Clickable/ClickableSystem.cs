// Decompiled with JetBrains decompiler
// Type: Content.Client.Clickable.ClickableSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Clickable;

public sealed class ClickableSystem : EntitySystem
{
  [Dependency]
  private IClickMapManager _clickMapManager;
  [Dependency]
  private SharedTransformSystem _transforms;
  [Dependency]
  private SpriteSystem _sprites;
  private EntityQuery<ClickableComponent> _clickableQuery;
  private EntityQuery<TransformComponent> _xformQuery;
  private EntityQuery<FadingSpriteComponent> _fadingSpriteQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._clickableQuery = this.GetEntityQuery<ClickableComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this._fadingSpriteQuery = this.GetEntityQuery<FadingSpriteComponent>();
  }

  public bool CheckClick(
    Entity<ClickableComponent?, SpriteComponent, TransformComponent?, FadingSpriteComponent?> entity,
    Vector2 worldPos,
    IEye eye,
    bool excludeFaded,
    out int drawDepth,
    out uint renderOrder,
    out float bottom)
  {
    if (!this._clickableQuery.Resolve(entity.Owner, ref entity.Comp1, false))
    {
      drawDepth = 0;
      renderOrder = 0U;
      bottom = 0.0f;
      return false;
    }
    if (!this._xformQuery.Resolve(entity.Owner, ref entity.Comp3, true))
    {
      drawDepth = 0;
      renderOrder = 0U;
      bottom = 0.0f;
      return false;
    }
    if (excludeFaded && this._fadingSpriteQuery.Resolve(entity.Owner, ref entity.Comp4, false))
    {
      drawDepth = 0;
      renderOrder = 0U;
      bottom = 0.0f;
      return false;
    }
    SpriteComponent comp2 = entity.Comp2;
    TransformComponent comp3 = entity.Comp3;
    if (!comp2.Visible)
    {
      drawDepth = 0;
      renderOrder = 0U;
      bottom = 0.0f;
      return false;
    }
    drawDepth = comp2.DrawDepth;
    renderOrder = comp2.RenderOrder;
    (Vector2 vector2_1, Angle angle1) = this._transforms.GetWorldPositionRotation(comp3);
    Box2Rotated bounds = this._sprites.CalculateBounds(Entity<SpriteComponent>.op_Implicit((entity.Owner, comp2)), vector2_1, angle1, eye.Rotation);
    bottom = Matrix3Helpers.TransformBox(Matrix3Helpers.CreateRotation(Angle.op_Implicit(eye.Rotation)), ref bounds).Bottom;
    Matrix3x2 result1;
    Matrix3x2.Invert(comp2.LocalMatrix, out result1);
    Angle angle2 = Angle.op_Addition(angle1, eye.Rotation);
    Angle angle3 = ((Angle) ref angle2).Reduced();
    Angle relativeRotation = ((Angle) ref angle3).FlipPositive();
    Angle angle4 = comp2.SnapCardinals ? DirectionExtensions.ToAngle(((Angle) ref relativeRotation).GetCardinalDir()) : Angle.Zero;
    ref Vector2 local1 = ref vector2_1;
    Angle angle5 = comp2.NoRotation ? Angle.op_UnaryNegation(eye.Rotation) : Angle.op_Subtraction(angle1, angle4);
    ref Angle local2 = ref angle5;
    Matrix3x2 inverseTransform = Matrix3Helpers.CreateInverseTransform(ref local1, ref local2);
    Vector2 vector2_2 = Vector2.Transform(Vector2.Transform(worldPos, inverseTransform), result1);
    if (this.CheckDirBound(Entity<ClickableComponent, SpriteComponent>.op_Implicit((entity.Owner, entity.Comp1, entity.Comp2)), relativeRotation, vector2_2))
      return true;
    foreach (ISpriteLayer allLayer in comp2.AllLayers)
    {
      if (allLayer is SpriteComponent.Layer layer && this._sprites.IsVisible(layer))
      {
        if (layer.Texture != null)
        {
          Vector2i pos = Vector2i.op_Explicit(vector2_2 * 32f * new Vector2(1f, -1f) + Vector2i.op_Division(layer.Texture.Size, 2f));
          if (this._clickMapManager.IsOccluding(layer.Texture, pos))
            return true;
        }
        Robust.Client.Graphics.RSI actualRsi = layer.ActualRsi;
        Robust.Client.Graphics.RSI.State state;
        if (actualRsi != null && actualRsi.TryGetState(layer.State, ref state))
        {
          RsiDirection rsiDirection = SpriteComponent.Layer.GetDirection(state.RsiDirections, relativeRotation);
          Matrix3x2 matrix;
          layer.GetLayerDrawMatrix(rsiDirection, ref matrix);
          Matrix3x2 result2;
          Matrix3x2.Invert(matrix, out result2);
          Vector2i pos = Vector2i.op_Explicit(Vector2.Transform(vector2_2, result2) * 32f * new Vector2(1f, -1f) + Vector2i.op_Division(state.Size, 2f));
          if (comp2.EnableDirectionOverride)
            rsiDirection = DirExt.Convert(comp2.DirectionOverride, state.RsiDirections);
          RsiDirection dir = DirExt.OffsetRsiDir(rsiDirection, layer.DirOffset);
          if (this._clickMapManager.IsOccluding(layer.ActualRsi, layer.State, dir, layer.AnimationFrame, pos))
            return true;
        }
      }
    }
    drawDepth = 0;
    renderOrder = 0U;
    bottom = 0.0f;
    return false;
  }

  public bool CheckDirBound(
    Entity<ClickableComponent, SpriteComponent> entity,
    Angle relativeRotation,
    Vector2 localPos)
  {
    ClickableComponent comp1 = entity.Comp1;
    SpriteComponent comp2 = entity.Comp2;
    if (comp1.Bounds == null)
      return false;
    Direction cardinalDir = ((Angle) ref relativeRotation).GetCardinalDir();
    Vector2 vector2_1;
    if (!comp2.NoRotation)
    {
      Angle angle = DirectionExtensions.ToAngle(cardinalDir);
      vector2_1 = ((Angle) ref angle).RotateVec(ref localPos);
    }
    else
      vector2_1 = localPos;
    Vector2 vector2_2 = vector2_1;
    if (((Box2) ref comp1.Bounds.All).Contains(vector2_2, true))
      return true;
    Box2 box2_1;
    switch (comp2.EnableDirectionOverride ? (int) comp2.DirectionOverride : (int) cardinalDir)
    {
      case 0:
        box2_1 = comp1.Bounds.South;
        break;
      case 2:
        box2_1 = comp1.Bounds.East;
        break;
      case 4:
        box2_1 = comp1.Bounds.North;
        break;
      case 6:
        box2_1 = comp1.Bounds.West;
        break;
      default:
        throw new InvalidOperationException();
    }
    Box2 box2_2 = box2_1;
    return ((Box2) ref box2_2).Contains(vector2_2, true);
  }
}
