// Decompiled with JetBrains decompiler
// Type: Content.Shared.Vehicle.VehicleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Vehicle.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Vehicle;

public sealed class VehicleSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedMoverController _mover;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.InitializeOperator();
    this.InitializeKey();
    this.SubscribeLocalEvent<VehicleComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<VehicleComponent, BeforeDamageChangedEvent>(this.OnBeforeDamageChanged));
    this.SubscribeLocalEvent<VehicleComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<VehicleComponent, UpdateCanMoveEvent>(this.OnVehicleUpdateCanMove));
    this.SubscribeLocalEvent<VehicleComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleComponent, ComponentShutdown>(this.OnVehicleShutdown));
    this.SubscribeLocalEvent<VehicleComponent, GetAdditionalAccessEvent>(new EntityEventRefHandler<VehicleComponent, GetAdditionalAccessEvent>(this.OnVehicleGetAdditionalAccess));
    this.SubscribeLocalEvent<VehicleOperatorComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleOperatorComponent, ComponentShutdown>(this.OnOperatorShutdown));
  }

  private void OnBeforeDamageChanged(
    Entity<VehicleComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    if (args.Cancelled || !ent.Comp.TransferDamage || !args.Damage.AnyPositive())
      return;
    EntityUid? nullable = ent.Comp.Operator;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    if (this.Transform(ent.Owner).MapID != this.Transform(valueOrDefault).MapID)
      return;
    DamageSpecifier damageSpec = DamageSpecifier.GetPositive(args.Damage);
    DamageModifierSet transferDamageModifier = ent.Comp.TransferDamageModifier;
    if (transferDamageModifier != null)
      damageSpec = DamageSpecifier.ApplyModifierSet(damageSpec, transferDamageModifier);
    DamageableSystem damageable = this._damageable;
    EntityUid? uid = new EntityUid?(valueOrDefault);
    DamageSpecifier damage = damageSpec;
    EntityUid? origin = args.Origin;
    nullable = new EntityUid?();
    EntityUid? tool = nullable;
    damageable.TryChangeDamage(uid, damage, origin: origin, tool: tool);
  }

  private void OnVehicleUpdateCanMove(Entity<VehicleComponent> ent, ref UpdateCanMoveEvent args)
  {
    VehicleCanRunEvent args1 = new VehicleCanRunEvent(ent);
    this.RaiseLocalEvent<VehicleCanRunEvent>((EntityUid) ent, ref args1);
    if (args1.CanRun)
      return;
    args.Cancel();
  }

  private void OnVehicleShutdown(Entity<VehicleComponent> ent, ref ComponentShutdown args)
  {
    this.TryRemoveOperator(ent);
  }

  private void OnVehicleGetAdditionalAccess(
    Entity<VehicleComponent> ent,
    ref GetAdditionalAccessEvent args)
  {
    EntityUid? nullable = ent.Comp.Operator;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    args.Entities.Add(valueOrDefault);
  }

  private void OnOperatorShutdown(Entity<VehicleOperatorComponent> ent, ref ComponentShutdown args)
  {
    this.TryRemoveOperator((Entity<VehicleOperatorComponent>) ((EntityUid) ent, (VehicleOperatorComponent) ent));
  }

  public bool TrySetOperator(Entity<VehicleComponent> entity, EntityUid? uid, bool removeExisting = true)
  {
    if (!entity.Comp.Operator.HasValue && !uid.HasValue)
      return false;
    VehicleOperatorComponent comp1;
    if (this.TryComp<VehicleOperatorComponent>(uid, out comp1))
    {
      EntityUid? vehicle = comp1.Vehicle;
      EntityUid owner = entity.Owner;
      return vehicle.HasValue && vehicle.GetValueOrDefault() == owner;
    }
    EntityUid? nullable;
    if (!removeExisting)
    {
      nullable = entity.Comp.Operator;
      if (nullable.HasValue)
        return false;
    }
    if (uid.HasValue && !this.CanOperate(entity.AsNullable(), uid.Value))
      return false;
    EntityUid? OldOperator = entity.Comp.Operator;
    nullable = entity.Comp.Operator;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      VehicleOperatorComponent comp2;
      if (this.TryComp<VehicleOperatorComponent>(valueOrDefault, out comp2))
      {
        OnVehicleExitedEvent args = new OnVehicleExitedEvent(entity, valueOrDefault);
        this.RaiseLocalEvent<OnVehicleExitedEvent>(valueOrDefault, ref args);
        comp2.Vehicle = new EntityUid?();
        this.RemCompDeferred<VehicleOperatorComponent>(valueOrDefault);
        this.RemCompDeferred<RelayInputMoverComponent>(valueOrDefault);
        this.RemCompDeferred<GridVehicleOperatorComponent>(valueOrDefault);
      }
    }
    entity.Comp.Operator = uid;
    if (uid.HasValue)
    {
      VehicleOperatorComponent operatorComponent = this.AddComp<VehicleOperatorComponent>(uid.Value);
      operatorComponent.Vehicle = new EntityUid?(entity.Owner);
      this.Dirty(uid.Value, (IComponent) operatorComponent);
      if (entity.Comp.MovementKind == VehicleMovementKind.Standard)
      {
        this._mover.SetRelay(uid.Value, (EntityUid) entity);
      }
      else
      {
        bool flag;
        switch (entity.Comp.MovementKind)
        {
          case VehicleMovementKind.Grid:
          case VehicleMovementKind.Free:
          case VehicleMovementKind.Aircraft:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
        if (flag)
        {
          this.EnsureComp<GridVehicleMoverComponent>(entity.Owner);
          this.EnsureComp<GridVehicleOperatorComponent>(uid.Value);
          this.RemCompDeferred<RelayInputMoverComponent>(uid.Value);
          this.RemCompDeferred<MovementRelayTargetComponent>((EntityUid) entity);
        }
      }
      OnVehicleEnteredEvent args = new OnVehicleEnteredEvent(entity, uid.Value);
      this.RaiseLocalEvent<OnVehicleEnteredEvent>(uid.Value, ref args);
    }
    else
      this.RemCompDeferred<MovementRelayTargetComponent>((EntityUid) entity);
    this.RefreshCanRun((Entity<VehicleComponent>) ((EntityUid) entity, entity.Comp));
    VehicleOperatorSetEvent args1 = new VehicleOperatorSetEvent(uid, OldOperator);
    this.RaiseLocalEvent<VehicleOperatorSetEvent>((EntityUid) entity, ref args1);
    this.Dirty<VehicleComponent>(entity);
    return true;
  }

  public bool TryRemoveOperator(Entity<VehicleComponent> entity)
  {
    return this.TrySetOperator(entity, new EntityUid?());
  }

  public bool TryRemoveOperator(Entity<VehicleOperatorComponent?> operatorEntity)
  {
    VehicleComponent comp;
    return !this.Resolve<VehicleOperatorComponent>((EntityUid) operatorEntity, ref operatorEntity.Comp, false) || !this.TryComp<VehicleComponent>(operatorEntity.Comp.Vehicle, out comp) || this.TrySetOperator((Entity<VehicleComponent>) (operatorEntity.Comp.Vehicle.Value, comp), new EntityUid?());
  }

  public bool TryGetOperator(
    Entity<VehicleComponent?> entity,
    [NotNullWhen(true)] out Entity<VehicleOperatorComponent>? operatorEnt)
  {
    operatorEnt = new Entity<VehicleOperatorComponent>?();
    if (!this.Resolve<VehicleComponent>((EntityUid) entity, ref entity.Comp))
      return false;
    EntityUid? nullable = entity.Comp.Operator;
    if (!nullable.HasValue)
      return false;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    VehicleOperatorComponent comp;
    if (!this.TryComp<VehicleOperatorComponent>(valueOrDefault, out comp))
      return false;
    operatorEnt = new Entity<VehicleOperatorComponent>?((Entity<VehicleOperatorComponent>) (valueOrDefault, comp));
    return true;
  }

  public EntityUid? GetOperatorOrNull(Entity<VehicleComponent?> entity)
  {
    Entity<VehicleOperatorComponent>? operatorEnt;
    this.TryGetOperator(entity, out operatorEnt);
    Entity<VehicleOperatorComponent>? nullable = operatorEnt;
    return !nullable.HasValue ? new EntityUid?() : new EntityUid?((EntityUid) nullable.GetValueOrDefault());
  }

  public bool HasOperator(Entity<VehicleComponent?> entity)
  {
    return this.TryGetOperator(entity, out Entity<VehicleOperatorComponent>? _);
  }

  public bool CanOperate(Entity<VehicleComponent?> entity, EntityUid uid)
  {
    return this.Resolve<VehicleComponent>((EntityUid) entity, ref entity.Comp) && !this._entityWhitelist.IsWhitelistFail(entity.Comp.OperatorWhitelist, uid) && this._actionBlocker.CanConsciouslyPerformAction(uid);
  }

  public void RefreshCanRun(Entity<VehicleComponent?> entity)
  {
    if (this.TerminatingOrDeleted((EntityUid) entity) || !this.Resolve<VehicleComponent>((EntityUid) entity, ref entity.Comp))
      return;
    this._actionBlocker.UpdateCanMove((EntityUid) entity);
    this.UpdateAppearance((Entity<VehicleComponent>) ((EntityUid) entity, entity.Comp));
  }

  private void UpdateAppearance(Entity<VehicleComponent> entity)
  {
    AppearanceComponent comp1;
    if (!this.TryComp<AppearanceComponent>((EntityUid) entity, out comp1))
      return;
    InputMoverComponent comp2;
    if (this.TryComp<InputMoverComponent>((EntityUid) entity, out comp2))
      this._appearance.SetData((EntityUid) entity, (Enum) VehicleVisuals.CanRun, (object) comp2.CanMove, comp1);
    this._appearance.SetData((EntityUid) entity, (Enum) VehicleVisuals.HasOperator, (object) entity.Comp.Operator.HasValue, comp1);
  }

  public void InitializeKey()
  {
    this.SubscribeLocalEvent<GenericKeyedVehicleComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<GenericKeyedVehicleComponent, ContainerIsInsertingAttemptEvent>(this.OnGenericKeyedInsertAttempt));
    this.SubscribeLocalEvent<GenericKeyedVehicleComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<GenericKeyedVehicleComponent, EntInsertedIntoContainerMessage>(this.OnGenericKeyedEntInserted));
    this.SubscribeLocalEvent<GenericKeyedVehicleComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<GenericKeyedVehicleComponent, EntRemovedFromContainerMessage>(this.OnGenericKeyedEntRemoved));
    this.SubscribeLocalEvent<GenericKeyedVehicleComponent, VehicleCanRunEvent>(new EntityEventRefHandler<GenericKeyedVehicleComponent, VehicleCanRunEvent>(this.OnGenericKeyedCanRun));
  }

  private void OnGenericKeyedInsertAttempt(
    Entity<GenericKeyedVehicleComponent> ent,
    ref ContainerIsInsertingAttemptEvent args)
  {
    if (args.Cancelled || this._timing.ApplyingState || !ent.Comp.PreventInvalidInsertion || args.Container.ID != ent.Comp.ContainerId || this._entityWhitelist.IsWhitelistPass(ent.Comp.KeyWhitelist, args.EntityUid))
      return;
    args.Cancel();
  }

  private void OnGenericKeyedEntInserted(
    Entity<GenericKeyedVehicleComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    if (this._timing.ApplyingState || args.Container.ID != ent.Comp.ContainerId)
      return;
    this.RefreshCanRun((Entity<VehicleComponent>) ent.Owner);
  }

  private void OnGenericKeyedEntRemoved(
    Entity<GenericKeyedVehicleComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (this._timing.ApplyingState || args.Container.ID != ent.Comp.ContainerId)
      return;
    this.RefreshCanRun((Entity<VehicleComponent>) ent.Owner);
  }

  private void OnGenericKeyedCanRun(
    Entity<GenericKeyedVehicleComponent> ent,
    ref VehicleCanRunEvent args)
  {
    if (!args.CanRun)
      return;
    args.CanRun = false;
    BaseContainer container;
    if (!this._container.TryGetContainer(ent.Owner, ent.Comp.ContainerId, out container))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      if (!this._entityWhitelist.IsWhitelistFail(ent.Comp.KeyWhitelist, containedEntity))
      {
        args.CanRun = true;
        break;
      }
    }
  }

  public void InitializeOperator()
  {
    this.SubscribeLocalEvent<StrapVehicleComponent, StrappedEvent>(new EntityEventRefHandler<StrapVehicleComponent, StrappedEvent>(this.OnVehicleStrapped));
    this.SubscribeLocalEvent<StrapVehicleComponent, UnstrappedEvent>(new EntityEventRefHandler<StrapVehicleComponent, UnstrappedEvent>(this.OnVehicleUnstrapped));
    this.SubscribeLocalEvent<ContainerVehicleComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<ContainerVehicleComponent, EntInsertedIntoContainerMessage>(this.OnContainerEntInserted));
    this.SubscribeLocalEvent<ContainerVehicleComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<ContainerVehicleComponent, EntRemovedFromContainerMessage>(this.OnContainerEntRemoved));
  }

  private void OnVehicleStrapped(Entity<StrapVehicleComponent> ent, ref StrappedEvent args)
  {
    VehicleComponent comp;
    if (!this.TryComp<VehicleComponent>((EntityUid) ent, out comp))
      return;
    this.TrySetOperator((Entity<VehicleComponent>) ((EntityUid) ent, comp), new EntityUid?((EntityUid) args.Buckle));
  }

  private void OnVehicleUnstrapped(Entity<StrapVehicleComponent> ent, ref UnstrappedEvent args)
  {
    VehicleComponent comp;
    if (!this.TryComp<VehicleComponent>((EntityUid) ent, out comp))
      return;
    this.TrySetOperator((Entity<VehicleComponent>) ((EntityUid) ent, comp), new EntityUid?());
  }

  private void OnContainerEntInserted(
    Entity<ContainerVehicleComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    VehicleComponent comp;
    if (this._timing.ApplyingState || args.Container.ID != ent.Comp.ContainerId || !this.TryComp<VehicleComponent>((EntityUid) ent, out comp))
      return;
    this.TrySetOperator((Entity<VehicleComponent>) ((EntityUid) ent, comp), new EntityUid?(args.Entity), false);
  }

  private void OnContainerEntRemoved(
    Entity<ContainerVehicleComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    VehicleComponent comp;
    if (this._timing.ApplyingState || args.Container.ID != ent.Comp.ContainerId || !this.TryComp<VehicleComponent>((EntityUid) ent, out comp))
      return;
    EntityUid? nullable = comp.Operator;
    EntityUid entity = args.Entity;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entity ? 1 : 0) : 1) != 0)
      return;
    this.TryRemoveOperator((Entity<VehicleComponent>) ((EntityUid) ent, comp));
  }
}
