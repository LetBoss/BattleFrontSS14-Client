// Decompiled with JetBrains decompiler
// Type: Content.Shared.RatKing.SharedRatKingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.RatKing;

public abstract class SharedRatKingSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  protected IRobustRandom Random;
  [Dependency]
  private SharedActionsSystem _action;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RatKingComponent, ComponentStartup>(new ComponentEventHandler<RatKingComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<RatKingComponent, ComponentShutdown>(new ComponentEventHandler<RatKingComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<RatKingComponent, RatKingOrderActionEvent>(new ComponentEventHandler<RatKingComponent, RatKingOrderActionEvent>(this.OnOrderAction));
    this.SubscribeLocalEvent<RatKingServantComponent, ComponentShutdown>(new ComponentEventHandler<RatKingServantComponent, ComponentShutdown>(this.OnServantShutdown));
    this.SubscribeLocalEvent<RatKingRummageableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<RatKingRummageableComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetVerb));
    this.SubscribeLocalEvent<RatKingRummageableComponent, RatKingRummageDoAfterEvent>(new ComponentEventHandler<RatKingRummageableComponent, RatKingRummageDoAfterEvent>(this.OnDoAfterComplete));
  }

  private void OnStartup(EntityUid uid, RatKingComponent component, ComponentStartup args)
  {
    ActionsComponent comp;
    if (!this.TryComp<ActionsComponent>(uid, out comp))
      return;
    SharedActionsSystem action1 = this._action;
    EntityUid performer1 = uid;
    ref EntityUid? local1 = ref component.ActionRaiseArmyEntity;
    string actionRaiseArmy = component.ActionRaiseArmy;
    ActionsComponent actionsComponent1 = comp;
    EntityUid container1 = new EntityUid();
    ActionsComponent component1 = actionsComponent1;
    action1.AddAction(performer1, ref local1, actionRaiseArmy, container1, component1);
    SharedActionsSystem action2 = this._action;
    EntityUid performer2 = uid;
    ref EntityUid? local2 = ref component.ActionDomainEntity;
    string actionDomain = component.ActionDomain;
    ActionsComponent actionsComponent2 = comp;
    EntityUid container2 = new EntityUid();
    ActionsComponent component2 = actionsComponent2;
    action2.AddAction(performer2, ref local2, actionDomain, container2, component2);
    SharedActionsSystem action3 = this._action;
    EntityUid performer3 = uid;
    ref EntityUid? local3 = ref component.ActionOrderStayEntity;
    string actionOrderStay = component.ActionOrderStay;
    ActionsComponent actionsComponent3 = comp;
    EntityUid container3 = new EntityUid();
    ActionsComponent component3 = actionsComponent3;
    action3.AddAction(performer3, ref local3, actionOrderStay, container3, component3);
    SharedActionsSystem action4 = this._action;
    EntityUid performer4 = uid;
    ref EntityUid? local4 = ref component.ActionOrderFollowEntity;
    string actionOrderFollow = component.ActionOrderFollow;
    ActionsComponent actionsComponent4 = comp;
    EntityUid container4 = new EntityUid();
    ActionsComponent component4 = actionsComponent4;
    action4.AddAction(performer4, ref local4, actionOrderFollow, container4, component4);
    SharedActionsSystem action5 = this._action;
    EntityUid performer5 = uid;
    ref EntityUid? local5 = ref component.ActionOrderCheeseEmEntity;
    string actionOrderCheeseEm = component.ActionOrderCheeseEm;
    ActionsComponent actionsComponent5 = comp;
    EntityUid container5 = new EntityUid();
    ActionsComponent component5 = actionsComponent5;
    action5.AddAction(performer5, ref local5, actionOrderCheeseEm, container5, component5);
    SharedActionsSystem action6 = this._action;
    EntityUid performer6 = uid;
    ref EntityUid? local6 = ref component.ActionOrderLooseEntity;
    string actionOrderLoose = component.ActionOrderLoose;
    ActionsComponent actionsComponent6 = comp;
    EntityUid container6 = new EntityUid();
    ActionsComponent component6 = actionsComponent6;
    action6.AddAction(performer6, ref local6, actionOrderLoose, container6, component6);
    this.UpdateActions(uid, component);
  }

  private void OnShutdown(EntityUid uid, RatKingComponent component, ComponentShutdown args)
  {
    foreach (EntityUid servant in component.Servants)
    {
      RatKingServantComponent comp;
      if (this.TryComp<RatKingServantComponent>(servant, out comp))
        comp.King = new EntityUid?();
    }
    ActionsComponent comp1;
    if (!this.TryComp<ActionsComponent>(uid, out comp1))
      return;
    Entity<ActionsComponent> entity = new Entity<ActionsComponent>(uid, comp1);
    SharedActionsSystem action1 = this._action;
    Entity<ActionsComponent> performer1 = entity;
    EntityUid? actionRaiseArmyEntity = component.ActionRaiseArmyEntity;
    Entity<ActionComponent>? action2 = actionRaiseArmyEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) actionRaiseArmyEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action1.RemoveAction(performer1, action2);
    SharedActionsSystem action3 = this._action;
    Entity<ActionsComponent> performer2 = entity;
    EntityUid? nullable = component.ActionDomainEntity;
    Entity<ActionComponent>? action4 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action3.RemoveAction(performer2, action4);
    SharedActionsSystem action5 = this._action;
    Entity<ActionsComponent> performer3 = entity;
    nullable = component.ActionOrderStayEntity;
    Entity<ActionComponent>? action6 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action5.RemoveAction(performer3, action6);
    SharedActionsSystem action7 = this._action;
    Entity<ActionsComponent> performer4 = entity;
    nullable = component.ActionOrderFollowEntity;
    Entity<ActionComponent>? action8 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action7.RemoveAction(performer4, action8);
    SharedActionsSystem action9 = this._action;
    Entity<ActionsComponent> performer5 = entity;
    nullable = component.ActionOrderCheeseEmEntity;
    Entity<ActionComponent>? action10 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action9.RemoveAction(performer5, action10);
    SharedActionsSystem action11 = this._action;
    Entity<ActionsComponent> performer6 = entity;
    nullable = component.ActionOrderLooseEntity;
    Entity<ActionComponent>? action12 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action11.RemoveAction(performer6, action12);
  }

  private void OnOrderAction(
    EntityUid uid,
    RatKingComponent component,
    RatKingOrderActionEvent args)
  {
    if (component.CurrentOrder == args.Type)
      return;
    args.Handled = true;
    component.CurrentOrder = args.Type;
    this.Dirty(uid, (IComponent) component);
    this.DoCommandCallout(uid, component);
    this.UpdateActions(uid, component);
    this.UpdateAllServants(uid, component);
  }

  private void OnServantShutdown(
    EntityUid uid,
    RatKingServantComponent component,
    ComponentShutdown args)
  {
    RatKingComponent comp;
    if (!this.TryComp<RatKingComponent>(component.King, out comp))
      return;
    comp.Servants.Remove(uid);
  }

  private void UpdateActions(EntityUid uid, RatKingComponent? component = null)
  {
    if (!this.Resolve<RatKingComponent>(uid, ref component))
      return;
    SharedActionsSystem action1 = this._action;
    EntityUid? nullable = component.ActionOrderStayEntity;
    Entity<ActionComponent>? action2 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num1 = component.CurrentOrder == RatKingOrderType.Stay ? 1 : 0;
    action1.SetToggled(action2, num1 != 0);
    SharedActionsSystem action3 = this._action;
    nullable = component.ActionOrderFollowEntity;
    Entity<ActionComponent>? action4 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num2 = component.CurrentOrder == RatKingOrderType.Follow ? 1 : 0;
    action3.SetToggled(action4, num2 != 0);
    SharedActionsSystem action5 = this._action;
    nullable = component.ActionOrderCheeseEmEntity;
    Entity<ActionComponent>? action6 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num3 = component.CurrentOrder == RatKingOrderType.CheeseEm ? 1 : 0;
    action5.SetToggled(action6, num3 != 0);
    SharedActionsSystem action7 = this._action;
    nullable = component.ActionOrderLooseEntity;
    Entity<ActionComponent>? action8 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num4 = component.CurrentOrder == RatKingOrderType.Loose ? 1 : 0;
    action7.SetToggled(action8, num4 != 0);
    SharedActionsSystem action9 = this._action;
    nullable = component.ActionOrderStayEntity;
    Entity<ActionComponent>? action10 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action9.StartUseDelay(action10);
    SharedActionsSystem action11 = this._action;
    nullable = component.ActionOrderFollowEntity;
    Entity<ActionComponent>? action12 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action11.StartUseDelay(action12);
    SharedActionsSystem action13 = this._action;
    nullable = component.ActionOrderCheeseEmEntity;
    Entity<ActionComponent>? action14 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action13.StartUseDelay(action14);
    SharedActionsSystem action15 = this._action;
    nullable = component.ActionOrderLooseEntity;
    Entity<ActionComponent>? action16 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    action15.StartUseDelay(action16);
  }

  private void OnGetVerb(
    EntityUid uid,
    RatKingRummageableComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!this.HasComp<RatKingComponent>(args.User) || component.Looted)
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("rat-king-rummage-text");
    alternativeVerb.Priority = 0;
    alternativeVerb.Act = (Action) (() => this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, component.RummageDuration, (DoAfterEvent) new RatKingRummageDoAfterEvent(), new EntityUid?(uid), new EntityUid?(uid))
    {
      BlockDuplicate = true,
      BreakOnDamage = true,
      BreakOnMove = true,
      DistanceThreshold = new float?(2f)
    }));
    verbs.Add(alternativeVerb);
  }

  private void OnDoAfterComplete(
    EntityUid uid,
    RatKingRummageableComponent component,
    RatKingRummageDoAfterEvent args)
  {
    if (args.Cancelled || component.Looted)
      return;
    component.Looted = true;
    this.Dirty(uid, (IComponent) component);
    this._audio.PlayPredicted(component.Sound, uid, new EntityUid?(args.User));
    string prototype = this.PrototypeManager.Index<WeightedRandomEntityPrototype>(component.RummageLoot).Pick(this.Random);
    if (!this._net.IsServer)
      return;
    this.Spawn(prototype, this.Transform(uid).Coordinates);
  }

  public void UpdateAllServants(EntityUid uid, RatKingComponent component)
  {
    foreach (EntityUid servant in component.Servants)
      this.UpdateServantNpc(servant, component.CurrentOrder);
  }

  public virtual void UpdateServantNpc(EntityUid uid, RatKingOrderType orderType)
  {
  }

  public virtual void DoCommandCallout(EntityUid uid, RatKingComponent component)
  {
  }
}
