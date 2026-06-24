// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Orders.CivSquadLeaderOrdersSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Marines.Orders;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Orders;

public abstract class CivSquadLeaderOrdersSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private EvasionSystem _evasionSystem;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private IGameTiming _timing;
  private readonly HashSet<Entity<CivTeamMemberComponent>> _receivers = new HashSet<Entity<CivTeamMemberComponent>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CivSquadLeaderOrdersComponent, FocusActionEvent>(new EntityEventRefHandler<CivSquadLeaderOrdersComponent, FocusActionEvent>(this.OnFocusAction));
    this.SubscribeLocalEvent<CivSquadLeaderOrdersComponent, HoldActionEvent>(new EntityEventRefHandler<CivSquadLeaderOrdersComponent, HoldActionEvent>(this.OnHoldAction));
    this.SubscribeLocalEvent<CivSquadLeaderOrdersComponent, MoveActionEvent>(new EntityEventRefHandler<CivSquadLeaderOrdersComponent, MoveActionEvent>(this.OnMoveAction));
  }

  protected virtual void OnFocusAction(
    Entity<CivSquadLeaderOrdersComponent> orders,
    ref FocusActionEvent args)
  {
    if (!this.HandleAction<FocusOrderComponent>(orders))
      return;
    args.Handled = true;
  }

  protected virtual void OnHoldAction(
    Entity<CivSquadLeaderOrdersComponent> orders,
    ref HoldActionEvent args)
  {
    if (!this.HandleAction<HoldOrderComponent>(orders))
      return;
    args.Handled = true;
  }

  protected virtual void OnMoveAction(
    Entity<CivSquadLeaderOrdersComponent> orders,
    ref MoveActionEvent args)
  {
    if (!this.HandleAction<MoveOrderComponent>(orders))
      return;
    args.Handled = true;
  }

  private bool HandleAction<T>(Entity<CivSquadLeaderOrdersComponent> orders) where T : IOrderComponent, new()
  {
    CivTeamMemberComponent comp1;
    TransformComponent comp2;
    if (!this.TryComp<CivTeamMemberComponent>((EntityUid) orders, out comp1) || !this.TryComp((EntityUid) orders, out comp2) || this._mobState.IsDead((EntityUid) orders))
      return false;
    TimeSpan duration = orders.Comp.Duration * 2.0;
    SharedActionsSystem actions1 = this._actions;
    EntityUid? nullable = orders.Comp.FocusActionEntity;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    TimeSpan cooldown1 = orders.Comp.Cooldown;
    actions1.SetCooldown(action1, cooldown1);
    SharedActionsSystem actions2 = this._actions;
    nullable = orders.Comp.MoveActionEntity;
    Entity<ActionComponent>? action2 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    TimeSpan cooldown2 = orders.Comp.Cooldown;
    actions2.SetCooldown(action2, cooldown2);
    SharedActionsSystem actions3 = this._actions;
    nullable = orders.Comp.HoldActionEntity;
    Entity<ActionComponent>? action3 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    TimeSpan cooldown3 = orders.Comp.Cooldown;
    actions3.SetCooldown(action3, cooldown3);
    this._receivers.Clear();
    this._entityLookup.GetEntitiesInRange<CivTeamMemberComponent>(comp2.Coordinates, (float) orders.Comp.OrderRange, this._receivers);
    foreach (Entity<CivTeamMemberComponent> receiver in this._receivers)
    {
      if (receiver.Comp.TeamId == comp1.TeamId && !this._mobState.IsDead((EntityUid) receiver))
        this.AddOrder<T>((EntityUid) receiver, 1, duration);
    }
    return true;
  }

  private void AddOrder<T>(EntityUid receiver, int multiplier, TimeSpan duration) where T : IOrderComponent, new()
  {
    TimeSpan curTime = this._timing.CurTime;
    T obj = this.EnsureComp<T>(receiver);
    obj.Received.Add(((FixedPoint2) multiplier, curTime + duration));
    obj.Received.Sort((Comparison<(FixedPoint2, TimeSpan)>) ((a, b) => a.CompareTo(b)));
    this._movementSpeed.RefreshMovementSpeedModifiers(receiver);
    this._evasionSystem.RefreshEvasionModifiers(receiver);
  }
}
