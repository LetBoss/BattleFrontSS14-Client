// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Actions.SharedRMCActionsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Actions;

public abstract class SharedRMCActionsSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedInteractionSystem _interaction;
  private Robust.Shared.GameObjects.EntityQuery<ActionSharedCooldownComponent> _actionSharedCooldownQuery;

  public override void Initialize()
  {
    this._actionSharedCooldownQuery = this.GetEntityQuery<ActionSharedCooldownComponent>();
    this.SubscribeAllEvent<RMCMissedTargetActionEvent>(new EntityEventHandler<RMCMissedTargetActionEvent>(this.OnMissedTargetAction));
    this.SubscribeLocalEvent<ActionSharedCooldownComponent, ActionPerformedEvent>(new EntityEventRefHandler<ActionSharedCooldownComponent, ActionPerformedEvent>(this.OnSharedCooldownPerformed));
    this.SubscribeLocalEvent<ActionCooldownComponent, RMCActionUseEvent>(new EntityEventRefHandler<ActionCooldownComponent, RMCActionUseEvent>(this.OnCooldownUse));
    this.SubscribeLocalEvent<ActionInRangeUnobstructedComponent, RMCActionUseAttemptEvent>(new EntityEventRefHandler<ActionInRangeUnobstructedComponent, RMCActionUseAttemptEvent>(this.OnInRangeUnobstructedUseAttempt));
    this.SubscribeLocalEvent<ActionComponent, ActionReducedUseDelayEvent>(new ComponentEventHandler<ActionComponent, ActionReducedUseDelayEvent>(this.OnReducedUseDelayEvent));
  }

  private void OnMissedTargetAction(RMCMissedTargetActionEvent args)
  {
    EntityUid entity = this.GetEntity(args.Action);
    RMCCooldownOnMissComponent comp;
    if (!this.TryComp<RMCCooldownOnMissComponent>(entity, out comp))
      return;
    this._actions.SetIfBiggerCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) entity), comp.MissCooldown);
  }

  private void OnSharedCooldownPerformed(
    Entity<ActionSharedCooldownComponent> ent,
    ref ActionPerformedEvent args)
  {
    if (!ent.Comp.OnPerform)
      return;
    this.ActivateSharedCooldown((Entity<ActionSharedCooldownComponent>) ((EntityUid) ent, (ActionSharedCooldownComponent) ent), args.Performer);
  }

  public void ActivateSharedCooldown(
    Entity<ActionSharedCooldownComponent?> action,
    EntityUid performer)
  {
    if (!this.Resolve<ActionSharedCooldownComponent>((EntityUid) action, ref action.Comp, false) || action.Comp.Cooldown == TimeSpan.Zero)
      return;
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions(performer))
    {
      ActionSharedCooldownComponent component;
      if (this._actionSharedCooldownQuery.TryComp(entityUid, out component))
      {
        if (component.Id.HasValue)
        {
          EntProtoId? id1 = component.Id;
          EntProtoId? id2 = action.Comp.Id;
          if ((id1.HasValue == id2.HasValue ? (id1.HasValue ? (id1.GetValueOrDefault() == id2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
            goto label_8;
        }
        if (!action.Comp.Id.HasValue || !component.Ids.Contains(action.Comp.Id.Value))
          continue;
label_8:
        this._actions.SetIfBiggerCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid), action.Comp.Cooldown);
      }
    }
  }

  public void EnableSharedCooldownEvents(
    Entity<ActionSharedCooldownComponent?> action,
    EntityUid performer)
  {
    this.SetStatusOfSharedCooldownEvents(action, performer, true);
  }

  public void DisableSharedCooldownEvents(
    Entity<ActionSharedCooldownComponent?> action,
    EntityUid performer)
  {
    this.SetStatusOfSharedCooldownEvents(action, performer, false);
  }

  private void SetStatusOfSharedCooldownEvents(
    Entity<ActionSharedCooldownComponent?> action,
    EntityUid performer,
    bool newStatus)
  {
    if (!this.Resolve<ActionSharedCooldownComponent>((EntityUid) action, ref action.Comp, false) || action.Comp.Cooldown == TimeSpan.Zero)
      return;
    foreach ((EntityUid entityUid, ActionComponent comp) in this._actions.GetActions(performer))
    {
      ActionSharedCooldownComponent component;
      if (this._actionSharedCooldownQuery.TryComp(entityUid, out component))
      {
        if (component.Id.HasValue)
        {
          EntProtoId? id1 = component.Id;
          EntProtoId? id2 = action.Comp.Id;
          if ((id1.HasValue == id2.HasValue ? (id1.HasValue ? (id1.GetValueOrDefault() == id2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
            goto label_8;
        }
        if (!action.Comp.Id.HasValue || !component.Ids.Contains(action.Comp.Id.Value) && !component.ActiveIds.Contains(action.Comp.Id.Value))
          continue;
label_8:
        this._actions.SetEnabled(new Entity<ActionComponent>?((Entity<ActionComponent>) (entityUid, comp)), newStatus);
      }
    }
  }

  private void OnReducedUseDelayEvent(
    EntityUid uid,
    ActionComponent component,
    ActionReducedUseDelayEvent args)
  {
    ActionReducedUseDelayComponent comp1;
    if (!this.TryComp<ActionReducedUseDelayComponent>(uid, out comp1) || args.Amount < 0 || args.Amount > 1)
      return;
    comp1.UseDelayReduction = args.Amount;
    ActionSharedCooldownComponent comp2;
    if (this.TryComp<ActionSharedCooldownComponent>(uid, out comp2))
    {
      ActionReducedUseDelayComponent useDelayComponent = comp1;
      useDelayComponent.UseDelayBase.GetValueOrDefault();
      if (!useDelayComponent.UseDelayBase.HasValue)
      {
        TimeSpan cooldown = comp2.Cooldown;
        useDelayComponent.UseDelayBase = new TimeSpan?(cooldown);
      }
      this.RefreshSharedUseDelay((Entity<ActionReducedUseDelayComponent>) (uid, comp1), comp2);
    }
    else
    {
      ActionReducedUseDelayComponent useDelayComponent = comp1;
      if (!useDelayComponent.UseDelayBase.HasValue)
        useDelayComponent.UseDelayBase = component.UseDelay;
      this.RefreshUseDelay((Entity<ActionReducedUseDelayComponent>) (uid, comp1));
    }
  }

  private void RefreshUseDelay(Entity<ActionReducedUseDelayComponent> ent)
  {
    TimeSpan? useDelayBase = ent.Comp.UseDelayBase;
    if (!useDelayBase.HasValue)
      return;
    TimeSpan timeSpan = useDelayBase.GetValueOrDefault().Multiply(1.0 - ent.Comp.UseDelayReduction.Double());
    this._actions.SetUseDelay(new Entity<ActionComponent>?((Entity<ActionComponent>) ent.Owner), new TimeSpan?(timeSpan));
  }

  private void RefreshSharedUseDelay(
    Entity<ActionReducedUseDelayComponent> ent,
    ActionSharedCooldownComponent shared)
  {
    TimeSpan? useDelayBase = ent.Comp.UseDelayBase;
    if (!useDelayBase.HasValue)
      return;
    TimeSpan timeSpan = useDelayBase.GetValueOrDefault().Multiply(1.0 - ent.Comp.UseDelayReduction.Double());
    shared.Cooldown = timeSpan;
  }

  private void OnCooldownUse(Entity<ActionCooldownComponent> ent, ref RMCActionUseEvent args)
  {
    this._actions.SetIfBiggerCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) ent.Owner), ent.Comp.Cooldown);
  }

  private void OnInRangeUnobstructedUseAttempt(
    Entity<ActionInRangeUnobstructedComponent> ent,
    ref RMCActionUseAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    SharedInteractionSystem interaction = this._interaction;
    Entity<TransformComponent> user1 = (Entity<TransformComponent>) args.User;
    Entity<TransformComponent> other = (Entity<TransformComponent>) valueOrDefault;
    double range = (double) ent.Comp.Range;
    nullable = new EntityUid?();
    EntityUid? user2 = nullable;
    if (interaction.InRangeUnobstructed(user1, other, (float) range, user: user2))
      return;
    args.Cancelled = true;
  }

  public bool CanUseActionPopup(EntityUid user, EntityUid action, EntityUid? target = null)
  {
    RMCActionUseAttemptEvent args = new RMCActionUseAttemptEvent(user, target);
    this.RaiseLocalEvent<RMCActionUseAttemptEvent>(action, ref args);
    return !args.Cancelled;
  }

  private void ActionUsed(EntityUid user, EntityUid action)
  {
    RMCActionUseEvent args = new RMCActionUseEvent(user);
    this.RaiseLocalEvent<RMCActionUseEvent>(action, ref args);
  }

  public bool TryUseAction(EntityUid user, EntityUid action, EntityUid target)
  {
    if (!this.CanUseActionPopup(user, action, new EntityUid?(target)))
      return false;
    this.ActionUsed(user, action);
    return true;
  }

  public bool TryUseAction(InstantActionEvent action)
  {
    if (!this.CanUseActionPopup(action.Performer, (EntityUid) action.Action))
      return false;
    this.ActionUsed(action.Performer, (EntityUid) action.Action);
    return true;
  }

  public bool TryUseAction(EntityTargetActionEvent action)
  {
    if (!this.CanUseActionPopup(action.Performer, (EntityUid) action.Action, new EntityUid?(action.Target)))
      return false;
    this.ActionUsed(action.Performer, (EntityUid) action.Action);
    return true;
  }

  public bool TryUseAction(WorldTargetActionEvent action)
  {
    if (!this.CanUseActionPopup(action.Performer, (EntityUid) action.Action))
      return false;
    this.ActionUsed(action.Performer, (EntityUid) action.Action);
    return true;
  }

  public IEnumerable<Entity<ActionComponent>> GetActionsWithEvent<T>(EntityUid user) where T : BaseActionEvent
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions(user))
    {
      if (this._actions.GetEvent((EntityUid) action) is T)
        yield return action;
    }
  }

  public IEnumerable<Entity<ActionComponent, T>> GetActionsWithComp<T>(EntityUid user) where T : IComponent
  {
    SharedRMCActionsSystem rmcActionsSystem = this;
    foreach (Entity<ActionComponent> action in rmcActionsSystem._actions.GetActions(user))
    {
      T comp;
      if (rmcActionsSystem.TryComp<T>((EntityUid) action, out comp))
        yield return (Entity<ActionComponent, T>) ((EntityUid) action, (ActionComponent) action, comp);
    }
  }
}
