// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Invisibility.XenoInvisibilitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Invisibility;

public sealed class XenoInvisibilitySystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  private readonly HashSet<EntityUid> _contacts = new HashSet<EntityUid>();
  private Robust.Shared.GameObjects.EntityQuery<MarineComponent> _marineQuery;
  private Robust.Shared.GameObjects.EntityQuery<MobCollisionComponent> _mobCollisionQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoComponent> _xenoQuery;

  public override void Initialize()
  {
    this._marineQuery = this.GetEntityQuery<MarineComponent>();
    this._mobCollisionQuery = this.GetEntityQuery<MobCollisionComponent>();
    this._xenoQuery = this.GetEntityQuery<XenoComponent>();
    this.SubscribeLocalEvent<XenoTurnInvisibleComponent, XenoTurnInvisibleActionEvent>(new EntityEventRefHandler<XenoTurnInvisibleComponent, XenoTurnInvisibleActionEvent>(this.OnXenoTurnInvisibleAction));
    this.SubscribeLocalEvent<XenoActiveInvisibleComponent, ComponentRemove>(new EntityEventRefHandler<XenoActiveInvisibleComponent, ComponentRemove>(this.OnXenoActiveInvisibleRemove));
    this.SubscribeLocalEvent<XenoActiveInvisibleComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoActiveInvisibleComponent, MeleeHitEvent>(this.OnXenoActiveInvisibleMeleeHit));
    this.SubscribeLocalEvent<XenoActiveInvisibleComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>(new EntityEventRefHandler<XenoActiveInvisibleComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>(this.OnXenoDevourDoAfterAttempt));
    this.SubscribeLocalEvent<XenoActiveInvisibleComponent, XenoLeapHitEvent>(new EntityEventRefHandler<XenoActiveInvisibleComponent, XenoLeapHitEvent>(this.OnXenoActiveInvisibleLeapHit));
    this.SubscribeLocalEvent<XenoActiveInvisibleComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoActiveInvisibleComponent, RefreshMovementSpeedModifiersEvent>(this.OnXenoActiveInvisibleRefreshSpeed));
  }

  private void OnXenoTurnInvisibleAction(
    Entity<XenoTurnInvisibleComponent> xeno,
    ref XenoTurnInvisibleActionEvent args)
  {
    if (!this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    XenoActiveInvisibleComponent comp;
    if (this.TryComp<XenoActiveInvisibleComponent>((EntityUid) xeno, out comp))
    {
      TimeSpan refundedCooldown = this.GetRefundedCooldown(xeno, comp, xeno.Comp.ManualRefundMultiplier);
      this.RemoveInvisibility((Entity<XenoActiveInvisibleComponent>) ((EntityUid) xeno, comp), refundedCooldown);
    }
    else
    {
      XenoActiveInvisibleComponent invisibleComponent = this.EnsureComp<XenoActiveInvisibleComponent>((EntityUid) xeno);
      invisibleComponent.ExpiresAt = this._timing.CurTime + xeno.Comp.Duration;
      invisibleComponent.FullCooldown = xeno.Comp.FullCooldown;
      invisibleComponent.SpeedMultiplier = xeno.Comp.SpeedMultiplier;
      this.StartCooldown((Entity<XenoActiveInvisibleComponent>) ((EntityUid) xeno, invisibleComponent), xeno.Comp.ToggleLockoutTime, true);
      this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    }
  }

  private void OnXenoActiveInvisibleRemove(
    Entity<XenoActiveInvisibleComponent> xeno,
    ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) xeno))
      return;
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void OnXenoActiveInvisibleMeleeHit(
    Entity<XenoActiveInvisibleComponent> xeno,
    ref MeleeHitEvent args)
  {
    if (!args.IsHit || args.HitEntities.Count == 0)
      return;
    using (IEnumerator<EntityUid> enumerator = args.HitEntities.GetEnumerator())
    {
      if (!enumerator.MoveNext())
        return;
      EntityUid current = enumerator.Current;
      if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, current))
        return;
      this.RemoveInvisibility(xeno, xeno.Comp.FullCooldown);
    }
  }

  private void OnXenoDevourDoAfterAttempt(
    Entity<XenoActiveInvisibleComponent> xeno,
    ref DoAfterAttemptEvent<XenoDevourDoAfterEvent> args)
  {
    XenoTurnInvisibleComponent comp;
    if (!this.TryComp<XenoTurnInvisibleComponent>((EntityUid) xeno, out comp))
    {
      this.RemoveInvisibility(xeno, xeno.Comp.FullCooldown);
    }
    else
    {
      TimeSpan refundedCooldown = this.GetRefundedCooldown((Entity<XenoTurnInvisibleComponent>) ((EntityUid) xeno, comp), xeno.Comp, comp.RevealedRefundMultiplier);
      this.RemoveInvisibility(xeno, refundedCooldown);
    }
  }

  private void OnXenoActiveInvisibleLeapHit(
    Entity<XenoActiveInvisibleComponent> xeno,
    ref XenoLeapHitEvent args)
  {
    this.RemoveInvisibility(xeno, xeno.Comp.FullCooldown);
  }

  private void OnXenoActiveInvisibleRefreshSpeed(
    Entity<XenoActiveInvisibleComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    float num = xeno.Comp.SpeedMultiplier.Float();
    args.ModifySpeed(num, num);
  }

  private TimeSpan GetRefundedCooldown(
    Entity<XenoTurnInvisibleComponent> xeno,
    XenoActiveInvisibleComponent activeInvis,
    float refundMultiplier)
  {
    double num = (activeInvis.ExpiresAt - this._timing.CurTime) / xeno.Comp.Duration;
    return xeno.Comp.FullCooldown - num * (double) refundMultiplier * xeno.Comp.FullCooldown;
  }

  private void StartCooldown(
    Entity<XenoActiveInvisibleComponent> xeno,
    TimeSpan cooldownTime,
    bool toggledStatus)
  {
    foreach (Entity<ActionComponent> entity1 in this._rmcActions.GetActionsWithEvent<XenoTurnInvisibleActionEvent>((EntityUid) xeno))
    {
      Entity<ActionComponent> entity2 = entity1.AsNullable();
      this._actions.SetCooldown(new Entity<ActionComponent>?(entity2), cooldownTime);
      this._actions.SetToggled(new Entity<ActionComponent>?(entity2), toggledStatus);
    }
  }

  private void RemoveInvisibility(Entity<XenoActiveInvisibleComponent> xeno, TimeSpan cooldownTime)
  {
    this.RemCompDeferred<XenoActiveInvisibleComponent>((EntityUid) xeno);
    this.StartCooldown(xeno, cooldownTime, false);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    if (xeno.Comp.DidPopup)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-invisibility-expire"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    xeno.Comp.DidPopup = true;
    this.Dirty<XenoActiveInvisibleComponent>(xeno);
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoActiveInvisibleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoActiveInvisibleComponent>();
    EntityUid uid;
    XenoActiveInvisibleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.ExpiresAt <= curTime)
      {
        this.RemoveInvisibility((Entity<XenoActiveInvisibleComponent>) (uid, comp1), comp1.FullCooldown);
      }
      else
      {
        MobCollisionComponent component;
        if (this._mobCollisionQuery.TryComp(uid, out component) && component.Colliding)
        {
          this._contacts.Clear();
          this._physics.GetContactingEntities((Entity<PhysicsComponent>) uid, this._contacts);
          bool flag = false;
          foreach (EntityUid contact in this._contacts)
          {
            if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) uid, (Entity<HiveMemberComponent>) contact) && (this._marineQuery.HasComp(uid) || this._xenoQuery.HasComp(contact)))
              flag = true;
          }
          if (flag)
          {
            if (!comp1.DidPopup)
            {
              comp1.DidPopup = true;
              this.Dirty(uid, (IComponent) comp1);
              if (this._net.IsServer)
                this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-invisibility-expire-bump"), uid, uid, PopupType.MediumCaution);
            }
            this.RemoveInvisibility((Entity<XenoActiveInvisibleComponent>) (uid, comp1), comp1.FullCooldown);
          }
        }
      }
    }
  }
}
