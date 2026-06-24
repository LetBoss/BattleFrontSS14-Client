// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Orders.SharedMarineOrdersSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines.Orders;

public abstract class SharedMarineOrdersSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private EvasionSystem _evasionSystem;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private IGameTiming _timing;
  private readonly HashSet<Entity<MarineComponent>> _receivers = new HashSet<Entity<MarineComponent>>();
  private Robust.Shared.GameObjects.EntityQuery<MoveOrderArmorComponent> _moveOrderArmorQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._moveOrderArmorQuery = this.GetEntityQuery<MoveOrderArmorComponent>();
    this.SubscribeLocalEvent<MoveOrderComponent, EntityUnpausedEvent>(new EntityEventRefHandler<MoveOrderComponent, EntityUnpausedEvent>(this.OnUnpause<MoveOrderComponent>));
    this.SubscribeLocalEvent<FocusOrderComponent, EntityUnpausedEvent>(new EntityEventRefHandler<FocusOrderComponent, EntityUnpausedEvent>(this.OnUnpause<FocusOrderComponent>));
    this.SubscribeLocalEvent<HoldOrderComponent, EntityUnpausedEvent>(new EntityEventRefHandler<HoldOrderComponent, EntityUnpausedEvent>(this.OnUnpause<HoldOrderComponent>));
    this.SubscribeLocalEvent<MarineOrdersComponent, FocusActionEvent>(new EntityEventRefHandler<MarineOrdersComponent, FocusActionEvent>(this.OnAction));
    this.SubscribeLocalEvent<MarineOrdersComponent, HoldActionEvent>(new EntityEventRefHandler<MarineOrdersComponent, HoldActionEvent>(this.OnAction));
    this.SubscribeLocalEvent<MarineOrdersComponent, MoveActionEvent>(new EntityEventRefHandler<MarineOrdersComponent, MoveActionEvent>(this.OnAction));
    this.SubscribeLocalEvent<MoveOrderComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<MoveOrderComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovement));
    this.SubscribeLocalEvent<MoveOrderComponent, ComponentShutdown>(new EntityEventRefHandler<MoveOrderComponent, ComponentShutdown>(this.OnMoveShutdown));
    this.SubscribeLocalEvent<MoveOrderComponent, EvasionRefreshModifiersEvent>(new EntityEventRefHandler<MoveOrderComponent, EvasionRefreshModifiersEvent>(this.OnMoveOrderEvasionRefresh));
    this.SubscribeLocalEvent<MoveOrderComponent, DidEquipEvent>(new EntityEventRefHandler<MoveOrderComponent, DidEquipEvent>(this.OnMoveOrderDidEquip));
    this.SubscribeLocalEvent<MoveOrderComponent, DidUnequipEvent>(new EntityEventRefHandler<MoveOrderComponent, DidUnequipEvent>(this.OnMoveOrderDidUnequip));
    this.SubscribeLocalEvent<HoldOrderComponent, DamageModifyEvent>(new EntityEventRefHandler<HoldOrderComponent, DamageModifyEvent>(this.OnDamageModify));
  }

  private void OnDamageModify(Entity<HoldOrderComponent> orders, ref DamageModifyEvent args)
  {
    HoldOrderComponent comp = orders.Comp;
    if (comp.Received.Count == 0)
      return;
    Dictionary<string, FixedPoint2> damageDict = args.Damage.DamageDict;
    FixedPoint2 fixedPoint2_1 = (FixedPoint2) 1 - comp.DamageModifier * comp.Received[0].Multiplier;
    foreach (ProtoId<DamageTypePrototype> damageType in comp.DamageTypes)
    {
      FixedPoint2 fixedPoint2_2;
      if (damageDict.TryGetValue((string) damageType, out fixedPoint2_2))
        damageDict[(string) damageType] = fixedPoint2_2 * fixedPoint2_1;
    }
  }

  private void OnUnpause<T>(Entity<T> orders, ref EntityUnpausedEvent args) where T : IComponent, IOrderComponent
  {
    T comp = orders.Comp;
    for (int index = 0; index < comp.Received.Count; ++index)
    {
      (FixedPoint2 Multiplier, TimeSpan ExpiresAt) tuple = comp.Received[index];
      comp.Received[index] = (tuple.Multiplier, tuple.ExpiresAt + args.PausedTime);
    }
  }

  private void OnRefreshMovement(
    Entity<MoveOrderComponent> orders,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    MoveOrderComponent comp = orders.Comp;
    if (comp.Received.Count == 0)
      return;
    float num = 1f + (comp.MoveSpeedModifier * comp.Received[0].Multiplier).Float();
    args.ModifySpeed(num, num);
  }

  private void OnMoveShutdown(Entity<MoveOrderComponent> uid, ref ComponentShutdown ev)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) uid);
    this._evasionSystem.RefreshEvasionModifiers(uid.Owner);
  }

  private void OnMoveOrderEvasionRefresh(
    Entity<MoveOrderComponent> entity,
    ref EvasionRefreshModifiersEvent args)
  {
    if (entity.Owner != args.Entity.Owner || entity.Comp.Received.Count == 0)
      return;
    args.Evasion += entity.Comp.Received[0].Multiplier * entity.Comp.EvasionModifier;
  }

  private void OnMoveOrderDidEquip(Entity<MoveOrderComponent> ent, ref DidEquipEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private void OnMoveOrderDidUnequip(Entity<MoveOrderComponent> ent, ref DidUnequipEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  protected virtual void OnAction(Entity<MarineOrdersComponent> orders, ref FocusActionEvent args)
  {
    this.OnAction<FocusOrderComponent>(orders, (InstantActionEvent) args);
  }

  protected virtual void OnAction(Entity<MarineOrdersComponent> orders, ref HoldActionEvent args)
  {
    this.OnAction<HoldOrderComponent>(orders, (InstantActionEvent) args);
  }

  protected virtual void OnAction(Entity<MarineOrdersComponent> orders, ref MoveActionEvent args)
  {
    this.OnAction<MoveOrderComponent>(orders, (InstantActionEvent) args);
  }

  private void OnAction<T>(Entity<MarineOrdersComponent> orders, InstantActionEvent args) where T : IOrderComponent, new()
  {
    if (args.Handled || !this.HandleAction<T>(orders))
      return;
    args.Handled = true;
  }

  private bool HandleAction<T>(Entity<MarineOrdersComponent> orders) where T : IOrderComponent, new()
  {
    TransformComponent comp;
    if (!this.TryComp((EntityUid) orders, out comp) || this._mobState.IsDead((EntityUid) orders))
      return false;
    int multiplier = Math.Max(1, this._skills.GetSkill((Entity<SkillsComponent>) orders.Owner, orders.Comp.Skill));
    TimeSpan duration = orders.Comp.Duration * (double) (multiplier + 1);
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
    this._entityLookup.GetEntitiesInRange<MarineComponent>(comp.Coordinates, (float) orders.Comp.OrderRange, this._receivers);
    foreach (Entity<MarineComponent> receiver in this._receivers)
    {
      if (!this._mobState.IsDead((EntityUid) receiver))
        this.AddOrder<T>(receiver, multiplier, duration);
    }
    return true;
  }

  private void AddOrder<T>(Entity<MarineComponent> receiver, int multiplier, TimeSpan duration) where T : IOrderComponent, new()
  {
    TimeSpan curTime = this._timing.CurTime;
    T obj = this.EnsureComp<T>((EntityUid) receiver);
    obj.Received.Add(((FixedPoint2) multiplier, curTime + duration));
    obj.Received.Sort((Comparison<(FixedPoint2, TimeSpan)>) ((a, b) => a.CompareTo(b)));
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) receiver);
    this._evasionSystem.RefreshEvasionModifiers((EntityUid) receiver);
  }

  private void RemoveExpired<T>() where T : IComponent, IOrderComponent
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<T> entityQueryEnumerator = this.EntityQueryEnumerator<T>();
    TimeSpan curTime = this._timing.CurTime;
    EntityUid uid;
    T comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      for (int index = comp1.Received.Count - 1; index >= 0; --index)
      {
        if (comp1.Received[index].ExpiresAt < curTime)
          comp1.Received.RemoveAt(index);
      }
      if (comp1.Received.Count == 0)
        this.RemCompDeferred<T>(uid);
    }
  }

  public void StartActionUseDelay(Entity<MarineOrdersComponent> orders)
  {
    SharedActionsSystem actions1 = this._actions;
    EntityUid? nullable = orders.Comp.HoldActionEntity;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions1.StartUseDelay(action1);
    SharedActionsSystem actions2 = this._actions;
    nullable = orders.Comp.MoveActionEntity;
    Entity<ActionComponent>? action2 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions2.StartUseDelay(action2);
    SharedActionsSystem actions3 = this._actions;
    nullable = orders.Comp.FocusActionEntity;
    Entity<ActionComponent>? action3 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions3.StartUseDelay(action3);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    this.RemoveExpired<MoveOrderComponent>();
    this.RemoveExpired<FocusOrderComponent>();
    this.RemoveExpired<HoldOrderComponent>();
  }
}
