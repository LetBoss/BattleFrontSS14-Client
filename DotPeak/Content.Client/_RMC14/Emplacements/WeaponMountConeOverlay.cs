// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Emplacements.WeaponMountConeOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Emplacements;
using Content.Shared.Buckle.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Emplacements;

public sealed class WeaponMountConeOverlay : Overlay
{
  [Dependency]
  private readonly IEntityManager _entity;
  [Dependency]
  private readonly IPlayerManager _player;
  private readonly SharedTransformSystem _transform;
  private readonly SharedWeaponMountSystem _mount;
  private const float ConeLength = 50f;
  private const float DashLength = 0.4f;
  private const float GapLength = 0.3f;
  private static readonly Color LineColor;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public WeaponMountConeOverlay()
  {
    IoCManager.InjectDependencies<WeaponMountConeOverlay>(this);
    this._transform = this._entity.System<SharedTransformSystem>();
    this._mount = this._entity.System<SharedWeaponMountSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    BuckleComponent buckleComponent;
    if (!localEntity.HasValue || !this._entity.TryGetComponent<BuckleComponent>(localEntity.Value, ref buckleComponent) || !buckleComponent.Buckled)
      return;
    EntityUid entityUid = buckleComponent.BuckledTo.Value;
    int shootArc;
    if (!this._mount.TryGetMountCone(Entity<WeaponMountComponent>.op_Implicit(entityUid), out shootArc))
      return;
    (Vector2 origin, Angle angle1) = this._transform.GetWorldPositionRotation(entityUid);
    Angle angle2 = Angle.FromDegrees((double) shootArc / 2.0);
    Angle angle3 = Angle.op_Addition(angle1, angle2);
    Angle angle4 = Angle.op_Subtraction(angle1, angle2);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    WeaponMountConeOverlay.DrawDashedLine(worldHandle, origin, angle3);
    WeaponMountConeOverlay.DrawDashedLine(worldHandle, origin, angle4);
  }

  private static void DrawDashedLine(DrawingHandleWorld handle, Vector2 origin, Angle angle)
  {
    Vector2 worldVec = ((Angle) ref angle).ToWorldVec();
    float num1 = 0.700000048f;
    for (float num2 = 0.0f; (double) num2 < 50.0; num2 += num1)
    {
      Vector2 vector2_1 = origin + worldVec * num2;
      float num3 = MathF.Min(num2 + 0.4f, 50f);
      Vector2 vector2_2 = origin + worldVec * num3;
      ((DrawingHandleBase) handle).DrawLine(vector2_1, vector2_2, WeaponMountConeOverlay.LineColor);
    }
  }

  static WeaponMountConeOverlay()
  {
    Color white = Color.White;
    WeaponMountConeOverlay.LineColor = ((Color) ref white).WithAlpha(0.25f);
  }
}
