// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.VehicleTurretInputSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CombatMode;
using Content.Shared._RMC14.Vehicle;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleTurretInputSystem : EntitySystem
{
  private const float AimUpdateInterval = 0.1f;
  private static readonly Angle AimEpsilon = Angle.FromDegrees(1.0);
  [Dependency]
  private readonly CombatModeSystem _combat;
  [Dependency]
  private readonly IEyeManager _eye;
  [Dependency]
  private readonly IInputManager _input;
  [Dependency]
  private readonly IPlayerManager _player;
  [Dependency]
  private readonly VehicleTurretSystem _turrets;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly IGameTiming _timing;
  private readonly Dictionary<EntityUid, (Angle Angle, TimeSpan Time)> _lastAims = new Dictionary<EntityUid, (Angle, TimeSpan)>();
  private readonly Dictionary<EntityUid, MapCoordinates> _lastAimCoordinates = new Dictionary<EntityUid, MapCoordinates>();

  public virtual void Update(float frameTime)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityUid? nullable = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    VehicleWeaponsOperatorComponent operatorComponent;
    if (!this._combat.IsInCombatMode(new EntityUid?(valueOrDefault1)) || !this.TryComp<VehicleWeaponsOperatorComponent>(valueOrDefault1, ref operatorComponent))
      return;
    nullable = operatorComponent.Vehicle;
    if (!nullable.HasValue)
      return;
    nullable = operatorComponent.SelectedWeapon;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
    VehicleTurretComponent vehicleTurretComponent;
    EntityUid targetUid;
    VehicleTurretComponent targetTurret;
    EntityCoordinates origin;
    if (!this.TryComp<VehicleTurretComponent>(valueOrDefault2, ref vehicleTurretComponent) || !this._turrets.TryResolveRotationTarget(valueOrDefault2, out targetUid, out targetTurret) || !targetTurret.RotateToCursor || !this._turrets.TryGetTurretOrigin(targetUid, targetTurret, out origin))
      return;
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return;
    this._lastAimCoordinates[valueOrDefault2] = map;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(origin, true);
    Vector2 vector2 = map.Position - mapCoordinates.Position;
    if ((double) vector2.LengthSquared() <= 9.9999997473787516E-05)
      return;
    Angle worldAngle = DirectionExtensions.ToWorldAngle(vector2);
    (Angle Angle, TimeSpan Time) tuple;
    if (this._lastAims.TryGetValue(targetUid, out tuple) && (this._timing.CurTime - tuple.Time).TotalSeconds < 0.10000000149011612)
    {
      Angle angle = Angle.ShortestDistance(ref worldAngle, ref tuple.Angle);
      if (Math.Abs(((Angle) ref angle).Degrees) < ((Angle) ref VehicleTurretInputSystem.AimEpsilon).Degrees)
        return;
    }
    this._lastAims[targetUid] = (worldAngle, this._timing.CurTime);
    EntityCoordinates coordinates = this._transform.ToCoordinates(map);
    this.RaisePredictiveEvent<VehicleTurretRotateEvent>(new VehicleTurretRotateEvent()
    {
      Turret = this.GetNetEntity(valueOrDefault2, (MetaDataComponent) null),
      Coordinates = this.GetNetCoordinates(coordinates, (MetaDataComponent) null)
    });
  }

  public bool TryGetLastAimCoordinates(EntityUid turretUid, out MapCoordinates coordinates)
  {
    return this._lastAimCoordinates.TryGetValue(turretUid, out coordinates);
  }
}
