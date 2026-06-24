// Decompiled with JetBrains decompiler
// Type: Content.Client.Decals.Overlays.DecalPlacementOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Decals.Overlays;

public sealed class DecalPlacementOverlay : Overlay
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IMapManager _mapManager;
  private readonly DecalPlacementSystem _placement;
  private readonly SharedTransformSystem _transform;
  private readonly SpriteSystem _sprite;

  public virtual OverlaySpace Space => (OverlaySpace) 16 /*0x10*/;

  public DecalPlacementOverlay(
    DecalPlacementSystem placement,
    SharedTransformSystem transform,
    SpriteSystem sprite)
  {
    IoCManager.InjectDependencies<DecalPlacementOverlay>(this);
    this._placement = placement;
    this._transform = transform;
    this._sprite = sprite;
    this.ZIndex = new int?(1000);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    (DecalPrototype Decal, bool Snap, Angle Angle, Color Color) = this._placement.GetActiveDecal();
    if (Decal == null)
      return;
    MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    if (MapId.op_Inequality(map.MapId, args.MapId) || !this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent))
      return;
    Matrix3x2 worldMatrix = this._transform.GetWorldMatrix(entityUid);
    Matrix3x2 invWorldMatrix = this._transform.GetInvWorldMatrix(entityUid);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle).SetTransform(ref worldMatrix);
    Vector2 vector2 = Vector2.Transform(map.Position, invWorldMatrix);
    if (Snap)
      vector2 = Vector2i.op_Implicit(Vector2Helpers.Floored(vector2)) + mapGridComponent.TileSizeHalfVector;
    Box2 box2 = ((Box2) ref Box2.UnitCentered).Translated(vector2);
    Box2Rotated box2Rotated;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref box2Rotated).\u002Ector(box2, Angle, vector2);
    worldHandle.DrawTextureRect(this._sprite.Frame0(Decal.Sprite), ref box2Rotated, new Color?(Color));
    Matrix3x2 identity = Matrix3x2.Identity;
    ((DrawingHandleBase) worldHandle).SetTransform(ref identity);
  }
}
