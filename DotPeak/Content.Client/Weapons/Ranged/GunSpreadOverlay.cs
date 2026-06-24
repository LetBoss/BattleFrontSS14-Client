// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.GunSpreadOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Ranged;

public sealed class GunSpreadOverlay : Overlay
{
  private IEntityManager _entManager;
  private readonly IEyeManager _eye;
  private readonly IGameTiming _timing;
  private readonly IInputManager _input;
  private readonly IPlayerManager _player;
  private readonly GunSystem _guns;
  private readonly SharedTransformSystem _transform;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public GunSpreadOverlay(
    IEntityManager entManager,
    IEyeManager eyeManager,
    IGameTiming timing,
    IInputManager input,
    IPlayerManager player,
    GunSystem system,
    SharedTransformSystem transform)
  {
    this._entManager = entManager;
    this._eye = eyeManager;
    this._input = input;
    this._timing = timing;
    this._player = player;
    this._guns = system;
    this._transform = transform;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    TransformComponent transformComponent;
    if (!localEntity.HasValue || !this._entManager.TryGetComponent<TransformComponent>(localEntity, ref transformComponent))
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(localEntity.Value, transformComponent);
    GunComponent gunComp;
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace) || !this._guns.TryGetGun(localEntity.Value, out EntityUid _, out gunComp))
      return;
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Inequality(mapCoordinates.MapId, map.MapId))
      return;
    Angle maxAngleModified = gunComp.MaxAngleModified;
    Angle minAngleModified = gunComp.MinAngleModified;
    double totalSeconds = (this._timing.CurTime - gunComp.NextFire).TotalSeconds;
    Angle angle1;
    // ISSUE: explicit constructor call
    ((Angle) ref angle1).\u002Ector(MathHelper.Clamp(gunComp.CurrentAngle.Theta - gunComp.AngleDecayModified.Theta * totalSeconds, gunComp.MinAngleModified.Theta, gunComp.MaxAngleModified.Theta));
    Vector2 vector2_1 = map.Position - mapCoordinates.Position;
    ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates.Position, map.Position + vector2_1, Color.Orange);
    ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates.Position, map.Position + ((Angle) ref maxAngleModified).RotateVec(ref vector2_1), Color.Red);
    DrawingHandleWorld drawingHandleWorld1 = worldHandle;
    Vector2 position1 = mapCoordinates.Position;
    Vector2 position2 = map.Position;
    Angle angle2 = Angle.op_UnaryNegation(maxAngleModified);
    Vector2 vector2_2 = ((Angle) ref angle2).RotateVec(ref vector2_1);
    Vector2 vector2_3 = position2 + vector2_2;
    Color red = Color.Red;
    ((DrawingHandleBase) drawingHandleWorld1).DrawLine(position1, vector2_3, red);
    ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates.Position, map.Position + ((Angle) ref minAngleModified).RotateVec(ref vector2_1), Color.Green);
    DrawingHandleWorld drawingHandleWorld2 = worldHandle;
    Vector2 position3 = mapCoordinates.Position;
    Vector2 position4 = map.Position;
    Angle angle3 = Angle.op_UnaryNegation(minAngleModified);
    Vector2 vector2_4 = ((Angle) ref angle3).RotateVec(ref vector2_1);
    Vector2 vector2_5 = position4 + vector2_4;
    Color green = Color.Green;
    ((DrawingHandleBase) drawingHandleWorld2).DrawLine(position3, vector2_5, green);
    ((DrawingHandleBase) worldHandle).DrawLine(mapCoordinates.Position, map.Position + ((Angle) ref angle1).RotateVec(ref vector2_1), Color.Yellow);
    DrawingHandleWorld drawingHandleWorld3 = worldHandle;
    Vector2 position5 = mapCoordinates.Position;
    Vector2 position6 = map.Position;
    Angle angle4 = Angle.op_UnaryNegation(angle1);
    Vector2 vector2_6 = ((Angle) ref angle4).RotateVec(ref vector2_1);
    Vector2 vector2_7 = position6 + vector2_6;
    Color yellow = Color.Yellow;
    ((DrawingHandleBase) drawingHandleWorld3).DrawLine(position5, vector2_7, yellow);
  }
}
