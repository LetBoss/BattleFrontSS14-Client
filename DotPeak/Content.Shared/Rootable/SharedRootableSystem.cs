// Decompiled with JetBrains decompiler
// Type: Content.Shared.Rootable.SharedRootableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.Coordinates;
using Content.Shared.Damage.Components;
using Content.Shared.Fluids.Components;
using Content.Shared.Gravity;
using Content.Shared.Mobs;
using Content.Shared.Movement.Systems;
using Content.Shared.Slippery;
using Content.Shared.Toggleable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Rootable;

public abstract class SharedRootableSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedGravitySystem _gravity;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private SharedAudioSystem _audio;
  protected Robust.Shared.GameObjects.EntityQuery<PuddleComponent> PuddleQuery;
  protected Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> PhysicsQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.PuddleQuery = this.GetEntityQuery<PuddleComponent>();
    this.PhysicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.SubscribeLocalEvent<RootableComponent, MapInitEvent>(new EntityEventRefHandler<RootableComponent, MapInitEvent>(this.OnRootableMapInit));
    this.SubscribeLocalEvent<RootableComponent, ComponentShutdown>(new EntityEventRefHandler<RootableComponent, ComponentShutdown>(this.OnRootableShutdown));
    this.SubscribeLocalEvent<RootableComponent, StartCollideEvent>(new EntityEventRefHandler<RootableComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<RootableComponent, EndCollideEvent>(new EntityEventRefHandler<RootableComponent, EndCollideEvent>(this.OnEndCollide));
    this.SubscribeLocalEvent<RootableComponent, ToggleActionEvent>(new EntityEventRefHandler<RootableComponent, ToggleActionEvent>(this.OnRootableToggle));
    this.SubscribeLocalEvent<RootableComponent, MobStateChangedEvent>(new EntityEventRefHandler<RootableComponent, MobStateChangedEvent>(this.OnMobStateChanged));
    this.SubscribeLocalEvent<RootableComponent, IsWeightlessEvent>(new EntityEventRefHandler<RootableComponent, IsWeightlessEvent>(this.OnIsWeightless));
    this.SubscribeLocalEvent<RootableComponent, SlipAttemptEvent>(new EntityEventRefHandler<RootableComponent, SlipAttemptEvent>(this.OnSlipAttempt));
    this.SubscribeLocalEvent<RootableComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<RootableComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovementSpeed));
  }

  private void OnRootableMapInit(Entity<RootableComponent> entity, ref MapInitEvent args)
  {
    ActionsComponent comp;
    if (!this.TryComp<ActionsComponent>((EntityUid) entity, out comp))
      return;
    entity.Comp.NextUpdate = this._timing.CurTime;
    SharedActionsSystem actions = this._actions;
    EntityUid performer = (EntityUid) entity;
    ref EntityUid? local = ref entity.Comp.ActionEntity;
    string action = (string) entity.Comp.Action;
    ActionsComponent actionsComponent = comp;
    EntityUid container = new EntityUid();
    ActionsComponent component = actionsComponent;
    actions.AddAction(performer, ref local, action, container, component);
  }

  private void OnRootableShutdown(Entity<RootableComponent> entity, ref ComponentShutdown args)
  {
    ActionsComponent comp;
    if (!this.TryComp<ActionsComponent>((EntityUid) entity, out comp))
      return;
    Entity<ActionsComponent> entity1 = new Entity<ActionsComponent>((EntityUid) entity, comp);
    SharedActionsSystem actions = this._actions;
    Entity<ActionsComponent> performer = entity1;
    EntityUid? actionEntity = entity.Comp.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) actionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions.RemoveAction(performer, action);
  }

  private void OnRootableToggle(Entity<RootableComponent> entity, ref ToggleActionEvent args)
  {
    args.Handled = this.TryToggleRooting((Entity<RootableComponent>) ((EntityUid) entity, (RootableComponent) entity));
  }

  private void OnMobStateChanged(Entity<RootableComponent> entity, ref MobStateChangedEvent args)
  {
    if (!entity.Comp.Rooted)
      return;
    this.TryToggleRooting((Entity<RootableComponent>) ((EntityUid) entity, (RootableComponent) entity));
  }

  public bool TryToggleRooting(Entity<RootableComponent?> entity)
  {
    if (!this.Resolve<RootableComponent>((EntityUid) entity, ref entity.Comp))
      return false;
    entity.Comp.Rooted = !entity.Comp.Rooted;
    this._movementSpeedModifier.RefreshMovementSpeedModifiers((EntityUid) entity);
    this.Dirty<RootableComponent>(entity);
    if (entity.Comp.Rooted)
    {
      this._alerts.ShowAlert((EntityUid) entity, entity.Comp.RootedAlert);
      TimeSpan curTime = this._timing.CurTime;
      if (curTime > entity.Comp.NextUpdate)
        entity.Comp.NextUpdate = curTime;
    }
    else
      this._alerts.ClearAlert((EntityUid) entity, entity.Comp.RootedAlert);
    this._audio.PlayPredicted(entity.Comp.RootSound, entity.Owner.ToCoordinates(), new EntityUid?((EntityUid) entity));
    return true;
  }

  private void OnIsWeightless(Entity<RootableComponent> ent, ref IsWeightlessEvent args)
  {
    if (args.Handled || !ent.Comp.Rooted || !this._gravity.EntityOnGravitySupportingGridOrMap((Entity<TransformComponent>) ent.Owner))
      return;
    args.IsWeightless = false;
    args.Handled = true;
  }

  private void OnSlipAttempt(Entity<RootableComponent> ent, ref SlipAttemptEvent args)
  {
    if (!ent.Comp.Rooted || args.SlipCausingEntity.HasValue && this.HasComp<DamageUserOnTriggerComponent>(args.SlipCausingEntity))
      return;
    args.NoSlip = true;
  }

  private void OnStartCollide(Entity<RootableComponent> entity, ref StartCollideEvent args)
  {
    if (!this.PuddleQuery.HasComp(args.OtherEntity))
      return;
    entity.Comp.PuddleEntity = new EntityUid?(args.OtherEntity);
    if (!(entity.Comp.NextUpdate < this._timing.CurTime))
      return;
    entity.Comp.NextUpdate = this._timing.CurTime;
  }

  private void OnEndCollide(Entity<RootableComponent> entity, ref EndCollideEvent args)
  {
    EntityUid? puddleEntity = entity.Comp.PuddleEntity;
    EntityUid otherEntity = args.OtherEntity;
    if ((puddleEntity.HasValue ? (puddleEntity.GetValueOrDefault() != otherEntity ? 1 : 0) : 1) != 0)
      return;
    bool flag = this.Exists(args.OtherEntity);
    PhysicsComponent component;
    if (!this.PhysicsQuery.TryComp((EntityUid) entity, out component))
      return;
    foreach (EntityUid contactingEntity in this._physics.GetContactingEntities((EntityUid) entity, component))
    {
      if ((!flag || !(contactingEntity == args.OtherEntity)) && this.PuddleQuery.HasComponent(contactingEntity))
      {
        entity.Comp.PuddleEntity = new EntityUid?(contactingEntity);
        return;
      }
    }
    entity.Comp.PuddleEntity = new EntityUid?();
  }

  private void OnRefreshMovementSpeed(
    Entity<RootableComponent> entity,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!entity.Comp.Rooted)
      return;
    args.ModifySpeed(entity.Comp.SpeedModifier);
  }
}
