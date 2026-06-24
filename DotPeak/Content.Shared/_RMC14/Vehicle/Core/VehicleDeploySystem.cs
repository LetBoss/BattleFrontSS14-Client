// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleDeploySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Sentry;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleDeploySystem : EntitySystem
{
  [Dependency]
  private readonly SharedActionsSystem _actions;
  [Dependency]
  private readonly SharedCMChatSystem _rmcChat;
  [Dependency]
  private readonly MetaDataSystem _meta;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly VehicleSystem _vehicleSystem;
  [Dependency]
  private readonly SharedCombatModeSystem _combatMode;
  [Dependency]
  private readonly SharedSentryTargetingSystem _targeting;
  [Dependency]
  private readonly SharedGunSystem _guns;
  [Dependency]
  private readonly VehicleTurretSystem _turret;
  [Dependency]
  private readonly VehicleTopologySystem _topology;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly SharedInteractionSystem _interaction;
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<StrapComponent, StrappedEvent>(new EntityEventRefHandler<StrapComponent, StrappedEvent>(this.OnDriverStrapped));
    this.SubscribeLocalEvent<StrapComponent, UnstrappedEvent>(new EntityEventRefHandler<StrapComponent, UnstrappedEvent>(this.OnDriverUnstrapped));
    this.SubscribeLocalEvent<VehicleDeployActionComponent, VehicleDeployActionEvent>(new EntityEventRefHandler<VehicleDeployActionComponent, VehicleDeployActionEvent>(this.OnDeployAction));
    this.SubscribeLocalEvent<VehicleDeployActionComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleDeployActionComponent, ComponentShutdown>(this.OnDeployActionShutdown));
    this.SubscribeLocalEvent<VehicleDeployableComponent, VehicleCanRunEvent>(new EntityEventRefHandler<VehicleDeployableComponent, VehicleCanRunEvent>(this.OnVehicleCanRun));
    this.SubscribeLocalEvent<HardpointSlotsChangedEvent>(new EntityEventHandler<HardpointSlotsChangedEvent>(this.OnHardpointSlotsChanged));
    this.SubscribeLocalEvent<HardpointItemComponent, AttemptShootEvent>(new EntityEventRefHandler<HardpointItemComponent, AttemptShootEvent>(this.OnDeployableAttemptShoot));
  }

  private void OnDriverStrapped(Entity<StrapComponent> ent, ref StrappedEvent args)
  {
    EntityUid? vehicle;
    VehicleDeployableComponent comp;
    if (this._net.IsClient || !this.HasComp<VehicleDriverSeatComponent>(ent.Owner) || !this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue || !this.TryComp<VehicleDeployableComponent>(vehicle.Value, out comp))
      return;
    this.EnableDeployAction(args.Buckle.Owner, vehicle.Value, comp);
  }

  private void OnDriverUnstrapped(Entity<StrapComponent> ent, ref UnstrappedEvent args)
  {
    EntityUid? vehicle;
    if (this._net.IsClient || !this.HasComp<VehicleDriverSeatComponent>(ent.Owner) || !this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return;
    this.DisableDeployAction(args.Buckle.Owner, vehicle.Value);
  }

  private void EnableDeployAction(
    EntityUid user,
    EntityUid vehicle,
    VehicleDeployableComponent deployable)
  {
    VehicleDeployActionComponent actionComp = this.EnsureComp<VehicleDeployActionComponent>(user);
    actionComp.Vehicle = new EntityUid?(vehicle);
    if (!actionComp.Action.HasValue)
      actionComp.Action = this._actions.AddAction(user, (string) actionComp.ActionId);
    this.UpdateDeployActionState(user, actionComp, deployable);
    this.Dirty(user, (IComponent) actionComp);
  }

  private void DisableDeployAction(EntityUid user, EntityUid vehicle)
  {
    VehicleDeployActionComponent comp;
    if (!this.TryComp<VehicleDeployActionComponent>(user, out comp))
      return;
    EntityUid? vehicle1 = comp.Vehicle;
    EntityUid entityUid = vehicle;
    if ((vehicle1.HasValue ? (vehicle1.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return;
    if (comp.Action.HasValue)
      this._actions.RemoveAction((Entity<ActionsComponent>) user, new Entity<ActionComponent>?((Entity<ActionComponent>) comp.Action.Value));
    this.RemCompDeferred<VehicleDeployActionComponent>(user);
  }

  private void OnDeployActionShutdown(
    Entity<VehicleDeployActionComponent> ent,
    ref ComponentShutdown args)
  {
    if (!ent.Comp.Action.HasValue)
      return;
    this._actions.RemoveAction((Entity<ActionsComponent>) ent.Owner, new Entity<ActionComponent>?((Entity<ActionComponent>) ent.Comp.Action.Value));
  }

  private void OnDeployAction(
    Entity<VehicleDeployActionComponent> ent,
    ref VehicleDeployActionEvent args)
  {
    if (this._net.IsClient || args.Handled || args.Performer != ent.Owner)
      return;
    args.Handled = true;
    EntityUid? vehicle = ent.Comp.Vehicle;
    if (!vehicle.HasValue)
      return;
    EntityUid valueOrDefault = vehicle.GetValueOrDefault();
    VehicleDeployableComponent comp1;
    if (!this.TryComp<VehicleDeployableComponent>(valueOrDefault, out comp1))
      return;
    VehicleComponent comp2;
    if (this.TryComp<VehicleComponent>(valueOrDefault, out comp2))
    {
      vehicle = comp2.Operator;
      EntityUid turretUid1 = ent.Owner;
      if ((vehicle.HasValue ? (vehicle.GetValueOrDefault() != turretUid1 ? 1 : 0) : 1) == 0)
      {
        if (comp1.Deploying)
          return;
        bool flag = !comp1.Deployed;
        if (flag && !this.TryGetVehicleTurret(valueOrDefault, out turretUid1))
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-deploy-requires-turret"), ent.Owner, new EntityUid?(ent.Owner), PopupType.SmallCaution);
          return;
        }
        comp1.Deploying = true;
        comp1.DeployingTo = flag;
        comp1.Deployer = new EntityUid?(ent.Owner);
        TimeSpan cooldown = flag ? comp1.DeployTime : comp1.UndeployTime;
        comp1.DeployEndTime = this._timing.CurTime + cooldown;
        comp1.AutoTarget = new EntityUid?();
        comp1.NextAutoTargetTime = TimeSpan.Zero;
        comp1.AutoSpinInitialized = false;
        this.Dirty(valueOrDefault, (IComponent) comp1);
        EntityUid turretUid2;
        if (!flag && this.TryGetVehicleTurret(valueOrDefault, out turretUid2))
        {
          Angle worldRotation = this._transform.GetWorldRotation(valueOrDefault);
          this._turret.TrySetTargetRotationWorld(turretUid2, worldRotation);
        }
        this.UpdateDeployActionState(ent.Owner, ent.Comp, comp1);
        EntityUid? action = ent.Comp.Action;
        if (action.HasValue)
          this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) action.Value), cooldown);
        string message = this.Loc.GetString(flag ? "rmc-vehicle-deploy-start" : "rmc-vehicle-undeploy-start");
        this._popup.PopupClient(message, ent.Owner, new EntityUid?(ent.Owner));
        this.SendDeployChat(ent.Owner, valueOrDefault, message);
        return;
      }
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-deploy-not-driver"), ent.Owner, new EntityUid?(ent.Owner), PopupType.SmallCaution);
  }

  private void OnVehicleCanRun(Entity<VehicleDeployableComponent> ent, ref VehicleCanRunEvent args)
  {
    if (!args.CanRun || !ent.Comp.Deploying && !ent.Comp.Deployed)
      return;
    args.CanRun = false;
  }

  private void UpdateDriverActionState(EntityUid vehicle, VehicleDeployableComponent deployable)
  {
    VehicleComponent comp1;
    if (!this.TryComp<VehicleComponent>(vehicle, out comp1) || !comp1.Operator.HasValue)
      return;
    EntityUid entityUid1 = comp1.Operator.Value;
    VehicleDeployActionComponent comp2;
    if (!this.TryComp<VehicleDeployActionComponent>(entityUid1, out comp2))
      return;
    EntityUid? vehicle1 = comp2.Vehicle;
    EntityUid entityUid2 = vehicle;
    if ((vehicle1.HasValue ? (vehicle1.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) != 0)
      return;
    this.UpdateDeployActionState(entityUid1, comp2, deployable);
    this.Dirty(entityUid1, (IComponent) comp2);
  }

  private void UpdateDeployActionState(
    EntityUid user,
    VehicleDeployActionComponent actionComp,
    VehicleDeployableComponent deployable)
  {
    if (!actionComp.Action.HasValue)
      return;
    bool flag = true;
    EntityUid? nullable1 = new EntityUid?();
    EntityUid? vehicle = actionComp.Vehicle;
    if (vehicle.HasValue)
    {
      EntityUid valueOrDefault1 = vehicle.GetValueOrDefault();
      EntityUid turretUid;
      bool vehicleTurret = this.TryGetVehicleTurret(valueOrDefault1, out turretUid);
      flag = deployable.Deployed | vehicleTurret;
      EntityUid? nullable2 = vehicleTurret ? new EntityUid?(turretUid) : new EntityUid?();
      EntityUid? action = actionComp.Action;
      if (action.HasValue)
      {
        EntityUid valueOrDefault2 = action.GetValueOrDefault();
        ActionComponent comp;
        if (this.TryComp<ActionComponent>(valueOrDefault2, out comp))
          this._actions.SetEntityIcon((Entity<ActionComponent>) (valueOrDefault2, comp), new EntityUid?(nullable2 ?? valueOrDefault1));
      }
    }
    EntityUid action1 = actionComp.Action.Value;
    this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) action1), deployable.Deployed || deployable.Deploying);
    this._actions.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) action1), !deployable.Deploying & flag);
    this.UpdateDeployActionText(action1, deployable);
  }

  private void UpdateDeployActionText(EntityUid action, VehicleDeployableComponent deployable)
  {
    string messageId1;
    string messageId2;
    if (deployable.Deploying)
    {
      if (deployable.DeployingTo)
      {
        messageId1 = "rmc-vehicle-deploy-action-name-deploying";
        messageId2 = "rmc-vehicle-deploy-action-desc-deploying";
      }
      else
      {
        messageId1 = "rmc-vehicle-deploy-action-name-undeploying";
        messageId2 = "rmc-vehicle-deploy-action-desc-undeploying";
      }
    }
    else if (deployable.Deployed)
    {
      messageId1 = "rmc-vehicle-deploy-action-name-undeploy";
      messageId2 = "rmc-vehicle-deploy-action-desc-undeploy";
    }
    else
    {
      messageId1 = "rmc-vehicle-deploy-action-name-deploy";
      messageId2 = "rmc-vehicle-deploy-action-desc-deploy";
    }
    this._meta.SetEntityName(action, this.Loc.GetString(messageId1));
    this._meta.SetEntityDescription(action, this.Loc.GetString(messageId2));
  }

  private void ClearDriverDeployCooldown(EntityUid vehicle)
  {
    VehicleComponent comp1;
    VehicleDeployActionComponent comp2;
    if (!this.TryComp<VehicleComponent>(vehicle, out comp1) || !comp1.Operator.HasValue || !this.TryComp<VehicleDeployActionComponent>(comp1.Operator.Value, out comp2) || !comp2.Action.HasValue)
      return;
    this._actions.ClearCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) comp2.Action.Value));
  }

  private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
  {
    VehicleDeployableComponent comp;
    if (this._net.IsClient || !this.TryComp<VehicleDeployableComponent>(args.Vehicle, out comp))
      return;
    this.UpdateDriverActionState(args.Vehicle, comp);
  }

  private void OnDeployableAttemptShoot(
    Entity<HardpointItemComponent> ent,
    ref AttemptShootEvent args)
  {
    if (this._net.IsClient && !this._timing.IsFirstTimePredicted || args.Cancelled || !string.Equals(ent.Comp.HardpointType, "Cannon", StringComparison.OrdinalIgnoreCase))
      return;
    EntityUid vehicle;
    if (!this.TryGetVehicleFromContained(ent.Owner, out vehicle))
    {
      args.Cancelled = true;
      args.ResetCooldown = true;
    }
    else
    {
      VehicleDeployableComponent comp1;
      VehicleDeployGatedHardpointsComponent comp2;
      if (!this.TryComp<VehicleDeployableComponent>(vehicle, out comp1) || !this.TryComp<VehicleDeployGatedHardpointsComponent>(vehicle, out comp2) || !VehicleDeploySystem.IsBlockedHardpoint(comp2, ent.Comp.HardpointType) || comp1.Deployed)
        return;
      args.Cancelled = true;
      args.ResetCooldown = true;
    }
  }

  private static bool IsBlockedHardpoint(
    VehicleDeployGatedHardpointsComponent gated,
    string hardpointType)
  {
    if (string.IsNullOrWhiteSpace(hardpointType))
      return false;
    foreach (string blockedHardpoint in gated.BlockedHardpoints)
    {
      if (string.Equals(blockedHardpoint, hardpointType, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleDeployableComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleDeployableComponent, TransformComponent>();
    EntityUid uid;
    VehicleDeployableComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out TransformComponent _))
    {
      TimeSpan curTime = this._timing.CurTime;
      if (comp1.Deploying)
      {
        if (curTime >= comp1.DeployEndTime)
        {
          bool deployingTo = comp1.DeployingTo;
          EntityUid? deployer = comp1.Deployer;
          comp1.Deploying = false;
          comp1.DeployingTo = false;
          comp1.DeployEndTime = TimeSpan.Zero;
          comp1.Deployed = deployingTo;
          if (!comp1.Deployed)
          {
            comp1.Deployer = new EntityUid?();
            comp1.TargetingDeployer = new EntityUid?();
            comp1.AutoTarget = new EntityUid?();
            comp1.AutoSpinInitialized = false;
          }
          this.Dirty(uid, (IComponent) comp1);
          this.UpdateDriverActionState(uid, comp1);
          this.ClearDriverDeployCooldown(uid);
          string messageId = deployingTo ? "rmc-vehicle-deploy-finish" : "rmc-vehicle-undeploy-finish";
          if (deployer.HasValue)
          {
            string message = this.Loc.GetString(messageId);
            this._popup.PopupClient(message, deployer.Value, new EntityUid?(deployer.Value));
            this.SendDeployChat(deployer.Value, uid, message);
          }
          else
            this._popup.PopupEntity(this.Loc.GetString(messageId), uid);
        }
      }
      else if (!comp1.Deployed || !comp1.AutoTurretEnabled)
      {
        comp1.AutoSpinInitialized = false;
      }
      else
      {
        EntityUid? entity = new EntityUid?();
        VehicleWeaponsComponent comp;
        if (this.TryComp<VehicleWeaponsComponent>(uid, out comp))
          entity = comp.Operator;
        if (entity.HasValue && this._combatMode.IsInCombatMode(entity))
        {
          comp1.AutoSpinInitialized = false;
        }
        else
        {
          EntityUid gunUid;
          GunComponent gunComp;
          if (this.TryFindAutoGun(uid, out gunUid, out gunComp))
          {
            if (comp1.AutoTarget.HasValue && !this.IsValidAutoTarget(uid, comp1, comp1.AutoTarget.Value, comp1.AutoTargetRange))
            {
              comp1.AutoTarget = new EntityUid?();
              comp1.NextAutoTargetTime = TimeSpan.Zero;
              comp1.AutoSpinInitialized = false;
            }
            if (!comp1.AutoTarget.HasValue && (comp1.NextAutoTargetTime == TimeSpan.Zero || curTime >= comp1.NextAutoTargetTime))
            {
              comp1.NextAutoTargetTime = curTime + TimeSpan.FromSeconds((double) Math.Max(0.0f, comp1.AutoTargetCooldown));
              comp1.AutoTarget = this.FindAutoTarget(uid, comp1, comp1.AutoTargetRange);
            }
            EntityUid? autoTarget = comp1.AutoTarget;
            if (autoTarget.HasValue)
            {
              EntityUid valueOrDefault = autoTarget.GetValueOrDefault();
              comp1.AutoSpinInitialized = false;
              EntityCoordinates targetCoords;
              if (!this._turret.TryAimAtTarget(gunUid, valueOrDefault, out targetCoords))
              {
                comp1.AutoTarget = new EntityUid?();
                comp1.NextAutoTargetTime = TimeSpan.Zero;
              }
              else if (this.HasAmmo(gunUid))
              {
                EntityUid? target = this._guns.SwapTarget((Entity<GunComponent>) (gunUid, gunComp), new EntityUid?(valueOrDefault));
                this._guns.AttemptShoot((Entity<GunComponent>) (gunUid, gunComp), uid, targetCoords);
                this._guns.SwapTarget((Entity<GunComponent>) (gunUid, gunComp), target);
              }
            }
            else if ((double) comp1.AutoSpinSpeed > 0.0)
            {
              if (!comp1.AutoSpinInitialized)
              {
                comp1.AutoSpinWorldRotation = this.GetTurretWorldRotation(gunUid, uid);
                comp1.AutoSpinInitialized = true;
              }
              Angle angle1 = Angle.FromDegrees((double) comp1.AutoSpinSpeed * (double) frameTime);
              VehicleDeployableComponent deployableComponent = comp1;
              Angle angle2 = Angle.op_Addition(comp1.AutoSpinWorldRotation, angle1);
              Angle angle3 = ((Angle) ref angle2).Reduced();
              deployableComponent.AutoSpinWorldRotation = angle3;
              this._turret.TrySetTargetRotationWorld(gunUid, comp1.AutoSpinWorldRotation);
            }
          }
        }
      }
    }
  }

  private bool TryFindAutoGun(EntityUid vehicle, out EntityUid gunUid, out GunComponent gunComp)
  {
    gunUid = new EntityUid();
    gunComp = (GunComponent) null;
    EntityUid? nullable1 = new EntityUid?();
    GunComponent gunComponent = (GunComponent) null;
    foreach (VehicleMountedSlot mountedSlot in this._topology.GetMountedSlots(vehicle))
    {
      EntityUid? nullable2 = mountedSlot.Item;
      EntityUid gunUid1;
      GunComponent gunComp1;
      if (nullable2.HasValue && this.TryGetGunCandidate(nullable2.GetValueOrDefault(), out gunUid1, out gunComp1))
      {
        if (this.HasAmmo(gunUid1))
        {
          gunUid = gunUid1;
          gunComp = gunComp1;
          return true;
        }
        nullable1.GetValueOrDefault();
        if (!nullable1.HasValue)
          nullable1 = new EntityUid?(gunUid1);
        if (gunComponent == null)
          gunComponent = gunComp1;
      }
    }
    if (!nullable1.HasValue || gunComponent == null)
      return false;
    gunUid = nullable1.Value;
    gunComp = gunComponent;
    return true;
  }

  private bool TryGetGunCandidate(EntityUid uid, out EntityUid gunUid, out GunComponent gunComp)
  {
    gunUid = uid;
    gunComp = (GunComponent) null;
    GunComponent comp;
    if (!this.TryComp<GunComponent>(uid, out comp) || !this.HasComp<VehicleTurretComponent>(uid))
      return false;
    gunComp = comp;
    return true;
  }

  private bool HasAmmo(EntityUid gunUid)
  {
    if (!this.HasComp<GunComponent>(gunUid))
      return false;
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>(gunUid, ref args);
    return args.Capacity <= 0 || args.Count > 0;
  }

  private EntityUid? FindAutoTarget(
    EntityUid vehicle,
    VehicleDeployableComponent deployable,
    float range)
  {
    if ((double) range <= 0.0)
      return new EntityUid?();
    SentryTargetingComponent targeting = this.EnsureComp<SentryTargetingComponent>(vehicle);
    if (deployable.Deployer.HasValue)
    {
      EntityUid? deployer = deployable.Deployer;
      EntityUid? targetingDeployer = deployable.TargetingDeployer;
      if ((deployer.HasValue == targetingDeployer.HasValue ? (deployer.HasValue ? (deployer.GetValueOrDefault() != targetingDeployer.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        this._targeting.ApplyDeployerFactions(vehicle, deployable.Deployer.Value);
        deployable.TargetingDeployer = deployable.Deployer;
      }
    }
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(vehicle);
    float num1 = float.MaxValue;
    EntityUid? autoTarget = new EntityUid?();
    foreach (EntityUid nearbyIffHostile in this._targeting.GetNearbyIffHostiles((Entity<SentryTargetingComponent>) (vehicle, targeting), range))
    {
      if (!(nearbyIffHostile == vehicle) && this.IsValidAutoTarget(vehicle, deployable, nearbyIffHostile, range, targeting))
      {
        float num2 = (this._transform.GetMapCoordinates(nearbyIffHostile).Position - mapCoordinates.Position).LengthSquared();
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          autoTarget = new EntityUid?(nearbyIffHostile);
        }
      }
    }
    return autoTarget;
  }

  private bool TryGetVehicleTurret(
    EntityUid vehicle,
    out EntityUid turretUid,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    return this._topology.TryGetPrimaryTurret(vehicle, out turretUid, hardpoints, itemSlots);
  }

  private bool TryGetVehicleFromContained(EntityUid contained, out EntityUid vehicle)
  {
    return this._topology.TryGetVehicle(contained, out vehicle);
  }

  private bool IsValidAutoTarget(
    EntityUid vehicle,
    VehicleDeployableComponent deployable,
    EntityUid target,
    float range)
  {
    SentryTargetingComponent comp;
    if (!this.TryComp<SentryTargetingComponent>(vehicle, out comp))
      comp = this.EnsureComp<SentryTargetingComponent>(vehicle);
    return this.IsValidAutoTarget(vehicle, deployable, target, range, comp);
  }

  private bool IsValidAutoTarget(
    EntityUid vehicle,
    VehicleDeployableComponent deployable,
    EntityUid target,
    float range,
    SentryTargetingComponent targeting)
  {
    if (deployable.Deployer.HasValue)
    {
      EntityUid? deployer = deployable.Deployer;
      EntityUid? targetingDeployer = deployable.TargetingDeployer;
      if ((deployer.HasValue == targetingDeployer.HasValue ? (deployer.HasValue ? (deployer.GetValueOrDefault() != targetingDeployer.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        this._targeting.ApplyDeployerFactions(vehicle, deployable.Deployer.Value);
        deployable.TargetingDeployer = deployable.Deployer;
      }
    }
    MobStateComponent comp1;
    if (!this.Exists(target) || this.TryComp<MobStateComponent>(target, out comp1) && comp1.CurrentState == MobState.Dead || !this._targeting.IsValidTarget((Entity<SentryTargetingComponent>) (vehicle, targeting), target))
      return false;
    EntityCoordinates coordinates = this.Transform(target).Coordinates;
    EntityUid turretUid;
    VehicleTurretComponent comp2;
    EntityCoordinates origin;
    if (this.TryGetVehicleTurret(vehicle, out turretUid) && this.TryComp<VehicleTurretComponent>(turretUid, out comp2) && this._turret.TryGetTurretOrigin(turretUid, comp2, out origin))
    {
      if (!this._interaction.InRangeUnobstructed(this._transform.ToMapCoordinates(origin), this._transform.ToMapCoordinates(coordinates), range))
        return false;
    }
    else if (!this._interaction.InRangeUnobstructed(vehicle, coordinates, range))
      return false;
    return true;
  }

  private Angle GetTurretWorldRotation(EntityUid turretUid, EntityUid vehicle)
  {
    VehicleTurretComponent comp;
    if (!this.TryComp<VehicleTurretComponent>(turretUid, out comp))
      return this._transform.GetWorldRotation(vehicle);
    Angle worldRotation = this._transform.GetWorldRotation(vehicle);
    Angle angle = Angle.op_Addition(comp.WorldRotation, worldRotation);
    return ((Angle) ref angle).Reduced();
  }

  private void SendDeployChat(EntityUid deployer, EntityUid vehicle, string message)
  {
    ActorComponent comp1;
    if (!this.TryComp<ActorComponent>(deployer, out comp1))
      return;
    MetaDataComponent comp2;
    string wrappedMessage = this.Loc.GetString("chat-manager-entity-say-wrap-message", ("entityName", (object) FormattedMessage.EscapeText(this.TryComp(vehicle, out comp2) ? comp2.EntityName : this.Loc.GetString("entity-unknown-name"))), ("verb", (object) this.Loc.GetString("chat-speech-verb-default")), ("fontType", (object) "Default"), ("fontSize", (object) 12), (nameof (message), (object) FormattedMessage.EscapeText(message)));
    this._rmcChat.ChatMessageToOne(ChatChannel.Local, message, wrappedMessage, vehicle, false, comp1.PlayerSession.Channel);
  }
}
