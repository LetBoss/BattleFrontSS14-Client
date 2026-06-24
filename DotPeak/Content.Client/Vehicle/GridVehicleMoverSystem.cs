// Decompiled with JetBrains decompiler
// Type: Content.Client.Vehicle.GridVehicleMoverSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared.Vehicle.Components;
using Robust.Client.Graphics;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Vehicle;

public sealed class GridVehicleMoverSystem : EntitySystem
{
  [Dependency]
  private readonly IConfigurationManager _cfg;
  [Dependency]
  private readonly IPlayerManager _playerManager;
  [Dependency]
  private readonly PhysicsSystem _physics;
  [Dependency]
  private readonly IOverlayManager _overlayManager;
  public static readonly List<Vector2> DebugCollisionPositions = new List<Vector2>();
  private GridVehicleMoverOverlay? _overlay;
  private VehicleHardpointDebugOverlay? _hardpointOverlay;
  private EntityUid? _lastPredictedVehicle;

  public virtual void Initialize()
  {
    this._overlay = new GridVehicleMoverOverlay((IEntityManager) this.EntityManager);
    this._overlay.DebugEnabled = this._cfg.GetCVar<bool>(RMCCVars.VehicleDebugOverlay);
    this._overlay.CollisionsEnabled = this._cfg.GetCVar<bool>(RMCCVars.VehicleCollisionOverlay);
    this._overlay.MovementEnabled = this._cfg.GetCVar<bool>(RMCCVars.VehicleMovementOverlay);
    this.RefreshSharedDebugFlags();
    this._hardpointOverlay = new VehicleHardpointDebugOverlay((IEntityManager) this.EntityManager)
    {
      Enabled = this._cfg.GetCVar<bool>(RMCCVars.VehicleHardpointOverlay)
    };
    this._cfg.OnValueChanged<bool>(RMCCVars.VehicleDebugOverlay, (Action<bool>) (val =>
    {
      if (this._overlay != null)
        this._overlay.DebugEnabled = val;
      this.RefreshSharedDebugFlags();
      this.RefreshVehicleDebugOverlay();
    }), true);
    this._cfg.OnValueChanged<bool>(RMCCVars.VehicleHardpointOverlay, (Action<bool>) (val =>
    {
      if (this._hardpointOverlay == null)
        return;
      this._hardpointOverlay.Enabled = val;
    }), true);
    this._cfg.OnValueChanged<bool>(RMCCVars.VehicleCollisionOverlay, (Action<bool>) (val =>
    {
      if (this._overlay != null)
        this._overlay.CollisionsEnabled = val;
      this.RefreshSharedDebugFlags();
      this.RefreshVehicleDebugOverlay();
    }), true);
    this._cfg.OnValueChanged<bool>(RMCCVars.VehicleMovementOverlay, (Action<bool>) (val =>
    {
      if (this._overlay != null)
        this._overlay.MovementEnabled = val;
      this.RefreshSharedDebugFlags();
      this.RefreshVehicleDebugOverlay();
    }), true);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GridVehicleMoverComponent, UpdateIsPredictedEvent>(new EntityEventRefHandler<GridVehicleMoverComponent, UpdateIsPredictedEvent>((object) this, __methodptr(OnUpdateIsPredicted)), (Type[]) null, (Type[]) null);
    this.RefreshVehicleDebugOverlay();
    this._overlayManager.AddOverlay((Overlay) this._hardpointOverlay);
  }

  public virtual void Shutdown()
  {
    if (this._overlay != null)
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    if (this._hardpointOverlay == null)
      return;
    this._overlayManager.RemoveOverlay((Overlay) this._hardpointOverlay);
  }

  private void RefreshSharedDebugFlags()
  {
    GridVehicleMoverOverlay overlay1 = this._overlay;
    Content.Shared.Vehicle.GridVehicleMoverSystem.CollisionDebugEnabled = overlay1 != null && (overlay1.DebugEnabled || overlay1.CollisionsEnabled);
    GridVehicleMoverOverlay overlay2 = this._overlay;
    Content.Shared.Vehicle.GridVehicleMoverSystem.MovementDebugEnabled = overlay2 != null && overlay2.MovementEnabled;
  }

  private void RefreshVehicleDebugOverlay()
  {
    if (this._overlay == null)
      return;
    bool flag1 = this._overlay.DebugEnabled || this._overlay.CollisionsEnabled || this._overlay.MovementEnabled;
    bool flag2 = this._overlayManager.HasOverlay<GridVehicleMoverOverlay>();
    if (flag1 && !flag2)
    {
      this._overlayManager.AddOverlay((Overlay) this._overlay);
    }
    else
    {
      if (!(!flag1 & flag2))
        return;
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    }
  }

  private void OnUpdateIsPredicted(
    Entity<GridVehicleMoverComponent> ent,
    ref UpdateIsPredictedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    VehicleComponent vehicleComponent;
    if (!this.TryComp<VehicleComponent>(ent.Owner, ref vehicleComponent))
      return;
    EntityUid? nullable = vehicleComponent.Operator;
    EntityUid entityUid = valueOrDefault;
    if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    args.IsPredicted = true;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (localEntity.HasValue)
    {
      VehicleOperatorComponent operatorComponent;
      EntityUid? nullable;
      if (this.TryComp<VehicleOperatorComponent>(localEntity.GetValueOrDefault(), ref operatorComponent))
      {
        nullable = operatorComponent.Vehicle;
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          nullable = this._lastPredictedVehicle;
          EntityUid entityUid = valueOrDefault;
          if ((nullable.HasValue ? (EntityUid.op_Inequality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) == 0)
            return;
          nullable = this._lastPredictedVehicle;
          if (nullable.HasValue)
            ((SharedPhysicsSystem) this._physics).UpdateIsPredicted(new EntityUid?(nullable.GetValueOrDefault()), (PhysicsComponent) null);
          this._lastPredictedVehicle = new EntityUid?(valueOrDefault);
          ((SharedPhysicsSystem) this._physics).UpdateIsPredicted(new EntityUid?(valueOrDefault), (PhysicsComponent) null);
          return;
        }
      }
      nullable = this._lastPredictedVehicle;
      if (!nullable.HasValue)
        return;
      ((SharedPhysicsSystem) this._physics).UpdateIsPredicted(new EntityUid?(nullable.GetValueOrDefault()), (PhysicsComponent) null);
      this._lastPredictedVehicle = new EntityUid?();
    }
    else
    {
      EntityUid? predictedVehicle = this._lastPredictedVehicle;
      if (predictedVehicle.HasValue)
        ((SharedPhysicsSystem) this._physics).UpdateIsPredicted(new EntityUid?(predictedVehicle.GetValueOrDefault()), (PhysicsComponent) null);
      this._lastPredictedVehicle = new EntityUid?();
    }
  }
}
