// Decompiled with JetBrains decompiler
// Type: Content.Client.Decals.Overlays.DecalOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Decals.Overlays;

public sealed class DecalOverlay : GridOverlay
{
  private readonly SpriteSystem _sprites;
  private readonly IEntityManager _entManager;
  private readonly IPrototypeManager _prototypeManager;
  private readonly Dictionary<string, (Texture Texture, bool SnapCardinals)> _cachedTextures = new Dictionary<string, (Texture, bool)>(64 /*0x40*/);
  private readonly List<(uint Id, Decal Decal)> _decals = new List<(uint, Decal)>();

  public DecalOverlay(
    SpriteSystem sprites,
    IEntityManager entManager,
    IPrototypeManager prototypeManager)
  {
    this._sprites = sprites;
    this._entManager = entManager;
    this._prototypeManager = prototypeManager;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (MapId.op_Equality(args.MapId, MapId.Nullspace))
      return;
    EntityUid owner = this.Grid.Owner;
    DecalGridComponent decalGridComponent;
    TransformComponent transformComponent;
    if (!this._entManager.TryGetComponent<DecalGridComponent>(owner, ref decalGridComponent) || !this._entManager.TryGetComponent<TransformComponent>(owner, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, args.MapId))
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    TransformSystem transformSystem = this._entManager.System<TransformSystem>();
    IEye eye = args.Viewport.Eye;
    Angle angle1 = eye != null ? eye.Rotation : Angle.Zero;
    Matrix3x2 invWorldMatrix = ((SharedTransformSystem) transformSystem).GetInvWorldMatrix(transformComponent);
    Box2Rotated box2Rotated = ((Box2Rotated) ref args.WorldBounds).Enlarged(1f);
    ref Box2Rotated local1 = ref box2Rotated;
    Box2 box2 = Matrix3Helpers.TransformBox(invWorldMatrix, ref local1);
    ChunkIndicesEnumerator indicesEnumerator;
    // ISSUE: explicit constructor call
    ((ChunkIndicesEnumerator) ref indicesEnumerator).\u002Ector(box2, 32 /*0x20*/);
    this._decals.Clear();
    Vector2i? nullable;
    while (((ChunkIndicesEnumerator) ref indicesEnumerator).MoveNext(ref nullable))
    {
      DecalGridComponent.DecalChunk decalChunk;
      if (decalGridComponent.ChunkCollection.ChunkCollection.TryGetValue(nullable.Value, out decalChunk))
      {
        foreach ((uint key, Decal decal) in decalChunk.Decals)
        {
          if (((Box2) ref box2).Contains(decal.Coordinates, true))
            this._decals.Add((key, decal));
        }
      }
    }
    if (this._decals.Count == 0)
      return;
    this._decals.Sort((Comparison<(uint, Decal)>) ((x, y) =>
    {
      int num = x.Decal.ZIndex.CompareTo(y.Decal.ZIndex);
      return num != 0 ? num : x.Id.CompareTo(y.Id);
    }));
    (Vector2 _, Angle angle2, Matrix3x2 matrix3x2) = ((SharedTransformSystem) transformSystem).GetWorldPositionRotationMatrix(transformComponent);
    ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
    foreach ((uint _, Decal Decal) in this._decals)
    {
      (Texture, bool) valueTuple;
      if (!this._cachedTextures.TryGetValue(Decal.Id, out valueTuple))
      {
        DecalPrototype decalPrototype;
        if (this._prototypeManager.TryIndex<DecalPrototype>(Decal.Id, ref decalPrototype))
        {
          valueTuple = (this._sprites.Frame0(decalPrototype.Sprite), decalPrototype.SnapCardinals);
          this._cachedTextures[Decal.Id] = valueTuple;
        }
        else
          continue;
      }
      Angle angle3 = Angle.Zero;
      if (valueTuple.Item2)
      {
        Angle angle4 = Angle.op_Addition(angle1, angle2);
        angle3 = DirectionExtensions.ToAngle(((Angle) ref angle4).GetCardinalDir());
      }
      Angle angle5 = Angle.op_Subtraction(Decal.Angle, angle3);
      if (((Angle) ref angle5).Equals(Angle.Zero))
        ((DrawingHandleBase) worldHandle).DrawTexture(valueTuple.Item1, Decal.Coordinates, Decal.Color);
      else
        worldHandle.DrawTexture(valueTuple.Item1, Decal.Coordinates, angle5, Decal.Color);
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local2 = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local2);
  }
}
