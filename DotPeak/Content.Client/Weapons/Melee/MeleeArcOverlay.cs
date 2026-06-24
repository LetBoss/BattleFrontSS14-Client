// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Melee.MeleeArcOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CombatMode;
using Content.Shared.Weapons.Melee;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Melee;

public sealed class MeleeArcOverlay : Overlay
{
  private readonly IEntityManager _entManager;
  private readonly IEyeManager _eyeManager;
  private readonly IInputManager _inputManager;
  private readonly IPlayerManager _playerManager;
  private readonly MeleeWeaponSystem _melee;
  private readonly SharedCombatModeSystem _combatMode;
  private readonly SharedTransformSystem _transform;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public MeleeArcOverlay(
    IEntityManager entManager,
    IEyeManager eyeManager,
    IInputManager inputManager,
    IPlayerManager playerManager,
    MeleeWeaponSystem melee,
    SharedCombatModeSystem combatMode,
    SharedTransformSystem transform)
  {
    this._entManager = entManager;
    this._eyeManager = eyeManager;
    this._inputManager = inputManager;
    this._playerManager = playerManager;
    this._melee = melee;
    this._combatMode = combatMode;
    this._transform = transform;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    TransformComponent transformComponent;
    MeleeWeaponComponent melee;
    if (!this._entManager.TryGetComponent<TransformComponent>(localEntity, ref transformComponent) || !this._combatMode.IsInCombatMode(localEntity) || !this._melee.TryGetWeapon(localEntity.Value, out EntityUid _, out melee))
      return;
    MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
    if (MapId.op_Inequality(map.MapId, args.MapId))
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(localEntity.Value, transformComponent);
    if (MapId.op_Inequality(map.MapId, mapCoordinates.MapId))
      return;
    Vector2 vector2_1 = map.Position - mapCoordinates.Position;
    if (vector2_1.Equals(Vector2.Zero))
      return;
    vector2_1 = Vector2Helpers.Normalized(vector2_1) * Math.Min(melee.Range, vector2_1.Length());
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawLine(mapCoordinates.Position, mapCoordinates.Position + vector2_1, Color.Aqua);
    if (melee.Angle.Theta == 0.0)
      return;
    DrawingHandleWorld worldHandle1 = ((OverlayDrawArgs) ref args).WorldHandle;
    Vector2 position1 = mapCoordinates.Position;
    Vector2 position2 = mapCoordinates.Position;
    Angle angle1 = new Angle(Angle.op_Implicit(Angle.op_UnaryNegation(melee.Angle)) / 2.0);
    Vector2 vector2_2 = ((Angle) ref angle1).RotateVec(ref vector2_1);
    Vector2 vector2_3 = position2 + vector2_2;
    Color orange1 = Color.Orange;
    ((DrawingHandleBase) worldHandle1).DrawLine(position1, vector2_3, orange1);
    DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs) ref args).WorldHandle;
    Vector2 position3 = mapCoordinates.Position;
    Vector2 position4 = mapCoordinates.Position;
    Angle angle2 = new Angle(Angle.op_Implicit(melee.Angle) / 2.0);
    Vector2 vector2_4 = ((Angle) ref angle2).RotateVec(ref vector2_1);
    Vector2 vector2_5 = position4 + vector2_4;
    Color orange2 = Color.Orange;
    ((DrawingHandleBase) worldHandle2).DrawLine(position3, vector2_5, orange2);
  }
}
